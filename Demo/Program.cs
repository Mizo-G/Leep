using MongoDB.Driver;
using MongoDB.Bson;

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

var connectionString = Environment.GetEnvironmentVariable("MONGODB_URI");

if (String.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("connection string is empty!");
    return;
}

var client = new MongoClient(connectionString);
var collection = client.GetDatabase("test").GetCollection<BsonDocument>("Users");

var filter = Builders<BsonDocument>.Filter.Empty;

app.MapGet("/weatherforecast", () =>
{
    Console.WriteLine("forecast looking gloomy");
    var documents = collection.Find(filter).ToList();

    foreach (var d in documents)
    {
        Console.WriteLine($"{d}");
    }

    return documents;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

