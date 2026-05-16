var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
