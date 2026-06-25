using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// UseRouting must be called before MapGet, MapPost, etc.
app.UseRouting();

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
 
app.ConfigureAwait(false);  
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

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
