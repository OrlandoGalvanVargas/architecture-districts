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
app.MapGet("/items", () => Results.Ok("List of items"));
app.MapGet("/items/search", (string query) => Results.Ok($"Search results for query: {query}"));    
app.MapGet("/items/filter", (string filter) => Results.Ok($"Filtered items with filter: {filter}"));
app.MapPost("/create", (string value) => Results.Ok($"Created item with value: {value}"));
app.MapPost("/items/batch", (string[] values) => Results.Ok($"Created batch of items with values: {string.Join(", ", values)}"));   
app.MapDelete("/items/{id}", (int id) => Results.Ok($"Deleted item with ID: {id}"));
app.MapGet("/items/{id}", (int id) => Results.Ok($"Item with ID: {id}"));
app.MapPut("", (int id, string value) => Results.Ok($"Updated item with ID: {id} to value: {value}"));
app.MapGet("/users/{userId}/orders/{orderId}"   , (int userId, int orderId) => Results.Ok($"User ID: {userId}, Order ID: {orderId}"));
app.MapPost("/users/{userId}/orders", (int userId, string orderDetails) => Results.Ok($"Created order for User ID: {userId} with details: {orderDetails}"));    
app.MapPut("/users/{userId}/orders/{orderId}", (int userId, int orderId, string orderDetails) => Results.Ok($"Updated order with ID: {orderId} for User ID: {userId} with details: {orderDetails}"));
app.MapDelete("/users/{userId}/orders/{orderId}", (int userId, int orderId) => Results.Ok($"Deleted order with ID: {orderId} for User ID: {userId}"));  
app.MapPatch("/users/{userId}/orders/{orderId}", (int userId, int orderId, string orderDetails) => Results.Ok($"Partially updated order with ID: {orderId} for User ID: {userId} with details: {orderDetails}"));   
app.MapGet("/products/{productId}/reviews", (int productId) => Results.Ok($"Reviews for Product ID: {productId}"));     
app.MapPost("/products/{productId}/reviews", (int productId, string reviewDetails) => Results.Ok($"Created review for Product ID: {productId} with details: {reviewDetails}"));     
app.MapDelete("/product/{productId}/reviews/{reviewId}" , (int productId, int reviewId) => Results.Ok($"Deleted review with ID: {reviewId} for Product ID: {productId}"));
app.MapGet("/search", (string query) => Results.Ok($"Search results for query: {query}"));       
app.MapPut("/products/{productId}"  , (int productId, string productDetails) => Results.Ok($"Updated product with ID: {productId} with details: {productDetails}"));
app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
