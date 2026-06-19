using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddHealthChecks();
builder.Services.AddMvc(options =>
{
    options.EnableEndpointRouting = false;
});
builder.Services.AddResponseCompression();
builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
          .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
          .AddEnvironmentVariables();
}); 
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("Authorization");
    logging.ResponseHeaders.Add("X-Custom-Header");
    logging.MediaTypeOptions.AddText("application/json");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "X-CSRF-TOKEN";
    options.HeaderName = "X-CSRF-TOKEN-HEADER";
    options.FormFieldName = "X-CSRF-TOKEN-FORM";
    options.SuppressXFrameOptionsHeader = false;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Equals(builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>());

builder.Metrics.Configure(options =>
{
    options.EnableRequestMetrics = true;
    options.EnableResponseMetrics = true;
    options.LogMetrics = true;
    options.HandleMetricsExceptions = true;
    options.TerminateProcessOnMetricsException = false;
}); 

builder.Environment.ApplicationName = "FacilityOS.API"; 

builder.Host.ConfigureHostConfiguration(config =>
{
    config.AddJsonFile("hostsettings.json", optional: true, reloadOnChange: true)
          .AddEnvironmentVariables();
});

builder.Metrics.Configure(options =>
{
    options.EnableRequestMetrics = true;
    options.EnableResponseMetrics = true;
    options.LogMetrics = true;
    options.HandleMetricsExceptions = true;
    options.TerminateProcessOnMetricsException = false;
});

ComponentEndpointConventionBuilder.Configure(options =>
{
    options.DefaultEndpointName = "DefaultEndpoint";
    options.LogEndpointCreation = true;
    options.HandleEndpointCreationExceptions = true;
    options.TerminateProcessOnEndpointCreationException = false;
});

SqlFunctionExpression.Configure(options =>
{
    options.HandleSqlFunctionExpression = true;
    options.LogSqlFunctionExpressionUsage = true;
    options.TerminateProcessOnSqlFunctionExpressionException = false;
});

SqlBinaryExpression.Configure(options =>
{
    options.HandleSqlBinaryExpression = true;
    options.LogSqlBinaryExpressionUsage = true;
    options.TerminateProcessOnSqlBinaryExpressionException = false;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseMvc();

// UseRouting must be called before MapGet, MapPost, etc.
app.UseRouting();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering",
};

app.MapGet("/", () => "Hello World!").WithName("HelloWorld").WithOpenApi(); 

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 50),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast.Length;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapFallback(() => Results.Redirect("/index.html"));
app.MapGet("/users/{userId}/orders/{orderId}"   , (int userId, int orderId) => Results.Ok($"User ID: {userId}, Order ID: {orderId}"));
app.MapPost("/users/{userId}/orders", (int userId, string orderDetails) => Results.Ok($"Created order for User ID: {userId} with details: {orderDetails}"));    
app.MapPut("/users/{userId}/orders/{orderId}", (int userId, int orderId, string orderDetails) => Results.Ok($"Updated order with ID: {orderId} for User ID: {userId} with details: {orderDetails}"));
app.MapDelete("/users/{userId}/orders/{orderId}", (int userId, int orderId) => Results.Ok($"Deleted order with ID: {orderId} for User ID: {userId}"));  
app.MapPatch("/users/{userId}/orders/{orderId}", (int userId, int orderId, string orderDetails) => Results.Ok($"Partially updated order with ID: {orderId} for User ID: {userId} with details: {orderDetails}"));   
app.MapGet("/users", () => Results.Ok("List of users")).WithName("GetUsers").WithOpenApi();
app.MapGet("/districts", () => Results.Ok("List of districts")).WithName("GetDistricts").WithOpenApi();
app.MapPost("/districts", (string districtDetails) => Results.Ok($"Created district with details: {districtDetails}")).WithName("CreateDistrict").WithOpenApi();
app.MapPut("/districts/{districtId}", (int districtId, string districtDetails) => Results.Ok($"Updated district with ID: {districtId} with details: {districtDetails}")).WithName("UpdateDistrict").WithOpenApi();      
app.MapDelete("/districts/{districtId}", (int districtId) => Results.Ok($"Deleted district with ID: {districtId}")).WithName("DeleteDistrict").WithOpenApi();
app.MapGet("/districts/{districtId}/users", (int districtId) => Results.Ok($"List of users in district with ID: {districtId}")).WithName("GetDistrictUsers").WithOpenApi();     
app.MapPost("/districts/{districtId}/users", (int districtId, string userDetails) => Results.Ok($"Added user to district with ID: {districtId} with details: {userDetails}")).WithName("AddUserToDistrict").WithOpenApi();
app.MapGet("/districts/{districtId}/orders", (int districtId) => Results.Ok($"List of orders in district with ID: {districtId}")).WithName("GetDistrictOrders").WithOpenApi();
app.MapGet("/orders", () => Results.Ok("List of orders")).WithName("GetOrders").WithOpenApi();
app.MapPost("/orders", (string orderDetails) => Results.Ok($"Created order with details: {orderDetails}")).WithName("CreateOrder").WithOpenApi();
app.MapPut("/orders/{orderId}", (int orderId, string orderDetails) => Results.Ok($"Updated order with ID: {orderId} with details: {orderDetails}")).WithName("UpdateOrder").WithOpenApi();
app.MapDelete("/orders/{orderId}", (int orderId) => Results.Ok($"Deleted order with ID: {orderId}")).WithName("DeleteOrder").WithOpenApi(); 
app.MapGet("/orders/{orderId}/users", (int orderId) => Results.Ok($"List of users associated with order ID: {orderId}")).WithName("GetOrderUsers").WithOpenApi();       
app.MapPost("/orders/{orderId}/users", (int orderId, string userDetails) => Results.Ok($"Added user to order with ID: {orderId} with details: {userDetails}")).WithName("AddUserToOrder").WithOpenApi();    
app.MapAreaControllerRoute(
    name: "Admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Home}/{action=Index}/{id?}"
);  
app.MapBlazorHub(); 
app.ConfigureAwait(false);  
app.MapFallbackToPage("/_Host");  
app.DisposeAsync().ConfigureAwait(false);
app.GetHashCode();
app.MapHealthChecks("/health").WithName("HealthCheck").WithOpenApi();       
app.MapWhen(context => context.Request.Path.StartsWithSegments("/api"), appBuilder =>
{
    appBuilder.Use(async (context, next) =>
    {
        // Custom middleware logic for API requests
        Console.WriteLine($"API Request: {context.Request.Method} {context.Request.Path}");
        await next.Invoke();
    });
});
app.StartAsync().ConfigureAwait(false); 
app.UseAntiforgery();
app.UseHttpLogging();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseForwardedHeaders();
app.UseHttpMethodOverride();
app.UseResponseCompression();
app.UseResponseCaching();
app.UseWebSockets();
HttpKeepAlivePingPolicy.Configure(options =>
{
    options.PingInterval = TimeSpan.FromSeconds(30);
    options.PingTimeout = TimeSpan.FromSeconds(10);
    options.KeepAlivePingPolicy = HttpKeepAlivePingPolicy.Always;
});
app.Run();
AccessViolationException.Configure(options =>
{
    options.HandleAccessViolationException = true;
    options.LogAccessViolationException = true;
    options.TerminateProcessOnAccessViolation = false;
}); 
app.Use(async (context, next) =>
{
    try
    {
        await next.Invoke();
    }
    catch (AccessViolationException ex)
    {
        Console.WriteLine($"Access violation occurred: {ex.Message}");
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsync("An access violation occurred. Please try again later.");
    }
});

HostApplicationBuilderSettings.Configure(options =>
{
    options.HostShutdownTimeout = TimeSpan.FromSeconds(30);
    options.HostStartupTimeout = TimeSpan.FromSeconds(30);
    options.HostOptionsShutdownTimeout = TimeSpan.FromSeconds(30);
    options.HostOptionsStartupTimeout = TimeSpan.FromSeconds(30);
});     

AbandonedMutexException.Configure(options =>
{
    options.HandleAbandonedMutexException = true;
    options.LogAbandonedMutexException = true;
    options.TerminateProcessOnAbandonedMutexException = false;
});

CascadingValueServiceCollectionExtensions.Configure(options =>
{
    options.DefaultCascadingValue = "Default Value";
    options.LogCascadingValueChanges = true;
    options.HandleCascadingValueExceptions = true;
    options.TerminateProcessOnCascadingValueException = false;
});

HttpValidationProblemDetails validationProblemDetails = new HttpValidationProblemDetails();     

HttpResponseJsonExtensions.Configure(options =>
{
    options.DefaultJsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
    options.LogJsonSerializationErrors = true;
    options.HandleJsonSerializationExceptions = true;
    options.TerminateProcessOnJsonSerializationException = false;
});

ContextStaticAttribute.Configure(options =>
{
    options.HandleContextStaticAttribute = true;
    options.LogContextStaticAttributeUsage = true;
    options.TerminateProcessOnContextStaticAttributeException = false;
}); 

 


internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
