using FacilityOS.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FacilityOS.API.Services;
using FacilityOS.API.Common.Exceptions;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using FluentValidation;
using FluentValidation.AspNetCore;
using FacilityOS.API.Validators;

var builder = WebApplication.CreateBuilder(args);

// 1. Servicios base
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateSchoolRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateSchoolRequestValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. Base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. MediatR — escanea automáticamente todos los Handlers en el ensamblado
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// 3.5 Servicios de la aplicacion (Injeccion de Dependencias)
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHealthChecks();
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("global", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });
});

// 4. Autenticación JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
        };
    }   );
builder.Services.AddAuthorization();

// 5. CORS — necesario para que React (puerto distinto) pueda llamar a la API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

var app = builder.Build();

// 6. Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseRateLimiter();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();