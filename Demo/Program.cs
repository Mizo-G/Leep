using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Demo.models;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Cosmos.Linq;

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

var connectionString = Environment.GetEnvironmentVariable("CosmosString");

if (String.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("connection string is empty!");
    return;
}

CosmosSerializationOptions serializerOptions = new()
{
    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
};

using CosmosClient client = new CosmosClientBuilder(connectionString)
    .WithSerializerOptions(serializerOptions)
    .Build();

var containter = client.GetContainer("shared", "Leep.User");

app.MapGet("/weatherforecast", async () =>
{
    Console.WriteLine("forecast looking gloomy");
    var item = new { id = "4", name = "forecast man", interests = new List<string>(["weather", "sadness"]) };
    var jitem = JsonConvert.SerializeObject(item);
    var res = await containter.UpsertItemAsync(jitem, new PartitionKey(item.id));
    
    Console.WriteLine(res.StatusCode);
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

