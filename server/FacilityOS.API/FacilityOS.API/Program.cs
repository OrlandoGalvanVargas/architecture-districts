using FacilityOS.API.Common;
using FacilityOS.API.Common.Behaviors;
using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Data;
using FacilityOS.API.Data.Interceptors;
using FacilityOS.API.Services;
using FacilityOS.API.Settings;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// 1. Servicios base
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

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

// 2. Base de datos
builder.Services.AddSingleton<UpdateAuditableEntitiesInterceptor>();

builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
{
    var interceptor = sp.GetRequiredService<UpdateAuditableEntitiesInterceptor>();

    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .AddInterceptors(interceptor);
});

// 3. MediatR con ValidationBehavior
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

// 4. Servicios de la aplicacion
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHealthChecks();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IResourceAuthorizationService, ResourceAuthorizationService>();

builder.Services.AddHostedService<TokenCleanupWorker>();

// 5. Carga de configuraciones tipadas
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
var rateLimitConfig = builder.Configuration.GetSection("RateLimiting");
int permitLimit = rateLimitConfig.GetValue<int>("PermitLimit", 100);
int windowMinutes = rateLimitConfig.GetValue<int>("WindowMinutes", 1);

if (string.IsNullOrWhiteSpace(jwtSettings.Key))
    throw new InvalidOperationException("Jwt:Key is not configured. Set it via user-secrets (dev) or environment variables (prod).");

if (string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("DefaultConnection")))
    throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not configured.");

if (builder.Environment.IsProduction())
{
    if (string.IsNullOrWhiteSpace(jwtSettings.Issuer))
        throw new InvalidOperationException("Jwt:Issuer is required in Production. Set via environment variables.");
    
    if (string.IsNullOrWhiteSpace(jwtSettings.Audience))
        throw new InvalidOperationException("Jwt:Audience is required in Production. Set via environment variables.");
    
    if (corsOrigins.Length == 0)
        throw new InvalidOperationException("Cors:AllowedOrigins is required in Production.");
    
    if (permitLimit <= 0)
        throw new InvalidOperationException("RateLimiting:PermitLimit must be greater than 0 in Production.");
}
else if (builder.Environment.IsDevelopment())
{
    if (string.IsNullOrWhiteSpace(jwtSettings.Issuer))
        Console.WriteLine("⚠️  WARNING (DEV): Jwt:Issuer is empty. Set via user-secrets for proper JWT generation.");
    
    if (string.IsNullOrWhiteSpace(jwtSettings.Audience))
        Console.WriteLine("⚠️  WARNING (DEV): Jwt:Audience is empty. Set via user-secrets for proper JWT generation.");
}

// 7. Rate Limiting
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

// 8. Autenticación JWT
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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Key)),
        };
    });

// Políticas de Autorización
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole(AppConstants.Roles.Admin));

    options.AddPolicy("DistrictAdminOrAbove", policy =>
        policy.RequireRole(AppConstants.Roles.Admin, AppConstants.Roles.DistrictAdmin));

    options.AddPolicy("SchoolAdminOrAbove", policy =>
        policy.RequireRole(AppConstants.Roles.Admin, AppConstants.Roles.DistrictAdmin, AppConstants.Roles.SchoolAdmin));
});

// 9. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(corsOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

var app = builder.Build();

// 10. Pipeline HTTP
app.UseMiddleware<ExceptionHandlingMiddleware>();

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

app.Run();
