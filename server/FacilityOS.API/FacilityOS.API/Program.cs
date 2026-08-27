using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Data;
using FacilityOS.API.Data.Interceptors;
using FacilityOS.API.Services;
using FacilityOS.Application.Common;
using FacilityOS.Application.Common.Behaviors;
using FacilityOS.Application.Common.Settings;
using FacilityOS.Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();


// 1. SERVICIOS BASE & VALIDACIÓN 
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = []
    });
});


// 2. PERSISTENCIA RESILIENTE & ALTO RENDIMIENTO
builder.Services.AddSingleton<UpdateAuditableEntitiesInterceptor>();

// Corrección de registro de DbContextPool
builder.Services.AddDbContextPool<ApplicationDbContext>((sp, options) =>
{
    var interceptor = sp.GetRequiredService<UpdateAuditableEntitiesInterceptor>();

    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    })
    .AddInterceptors(interceptor);
});

builder.Services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

// 3. MEDIATR CON COMPORTAMIENTOS ABIERTOS
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(ValidationBehavior<,>).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(typeof(ValidationBehavior<,>).Assembly);


// 4. SERVICIOS DE LA APLICACIÓN Y JOBS EN SEGUNDO PLANO
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddHealthChecks()
    .AddSqlServer(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "sqlserver",
        failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
        tags: new[] { "db", "data" });

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IResourceAuthorizationService, ResourceAuthorizationService>();
builder.Services.AddHostedService<TokenCleanupWorker>();

// 5. CARGA Y VALIDACIÓN DE CONFIGURACIONES
builder.Services.Configure<BCryptSettings>(builder.Configuration.GetSection(BCryptSettings.SectionName));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
var rateLimitConfig = builder.Configuration.GetSection("RateLimiting");
int permitLimit = rateLimitConfig.GetValue<int>("PermitLimit", 100);
int windowMinutes = rateLimitConfig.GetValue<int>("WindowMinutes", 1);

if (string.IsNullOrWhiteSpace(jwtSettings.Key) || jwtSettings.Key.Length < 32)
    throw new InvalidOperationException("Jwt:Key is not configured or is too short (min 32 characters for HMAC-SHA256).");

if (string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("DefaultConnection")))
    throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not configured.");

if (builder.Environment.IsProduction())
{
    if (string.IsNullOrWhiteSpace(jwtSettings.Issuer))
        throw new InvalidOperationException("Jwt:Issuer is required in Production.");

    if (string.IsNullOrWhiteSpace(jwtSettings.Audience))
        throw new InvalidOperationException("Jwt:Audience is required in Production.");

    if (corsOrigins.Length == 0)
        throw new InvalidOperationException("Cors:AllowedOrigins is required in Production.");

    if (permitLimit <= 0)
        throw new InvalidOperationException("RateLimiting:PermitLimit must be greater than 0 in Production.");
}

// 6. RATE LIMITING
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("global", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = permitLimit,
                Window = TimeSpan.FromMinutes(windowMinutes),
                QueueLimit = 0
            }));
});

// 7. SEGURIDAD: AUTENTICACIÓN JWT 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ClockSkew = TimeSpan.FromMinutes(5)
        };
    });

// 8. POLÍTICAS DE AUTORIZACIÓN JERÁRQUICA
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole(AppConstants.Roles.Admin));

    options.AddPolicy("DistrictAdminOrAbove", policy =>
        policy.RequireRole(AppConstants.Roles.Admin, AppConstants.Roles.DistrictAdmin));

    options.AddPolicy("SchoolAdminOrAbove", policy =>
        policy.RequireRole(AppConstants.Roles.Admin, AppConstants.Roles.DistrictAdmin, AppConstants.Roles.SchoolAdmin));
});

// 9. C.O.R.S RESTRICTIVO
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(corsOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

var app = builder.Build();

// APLICACIÓN AUTOMÁTICA DE MIGRACIONES EN PRODUCCIÓN (CI/CD)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        if (context.Database.GetPendingMigrations().Any())
        {
            Log.Information("New pending database migrations detected. Applying changes to SQL Server...");
            context.Database.Migrate();
            Log.Information("Database migrations applied successfully.");
        }
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "An error occurred while migrating the database during startup sequence.");
        throw;
    }
}

// 10. MIDDLEWARES
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSecurityHeaders();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "FacilityOS API v1");
        options.EnablePersistAuthorization();
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();

try
{
    Log.Information("Starting FacilityOS API Host...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly!");
}
finally
{
    Log.CloseAndFlush();
}