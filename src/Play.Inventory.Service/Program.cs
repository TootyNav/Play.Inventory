using MongoDB.Driver.Encryption;
using Play.Common.Repositories;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
using Polly;
using Polly.Timeout;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMongo()
    .AddMongoRepository<InventoryItem>("inventoryItem");

builder.Services.AddHttpClient<CatalogClient>(client =>
    {
        client.BaseAddress = new Uri("https://localhost:7003");
    })
    .AddTransientHttpErrorPolicy(x => x.Or<TimeoutRejectedException>()
        .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
    .AddTransientHttpErrorPolicy(x => x.Or<TimeoutRejectedException>()
        .CircuitBreakerAsync(3, TimeSpan.FromSeconds(15)))
    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));



builder.Services.AddControllers();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
