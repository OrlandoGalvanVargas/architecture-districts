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

app.MapPost("/create", (string value) => Results.Ok($"Created item with value: {value}"));
app.MapDelete("/delete", (string id) => Results.Ok($"Deleted item with ID: {id}"));

app.MapGet("/items/{id}", (int id) => Results.Ok($"Item with ID: {id}"));
app.MapPut("/items/{id}", (int id, string value) => Results.Ok($"Updated item with ID: {id} to value: {value}"));


app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
