using Vibey.Inventory.Api;
using Vibey.Inventory.Application;
using Vibey.Inventory.Domain;
using Vibey.Inventory.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddCors(o =>
  o.AddDefaultPolicy(p => p.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod())
);

builder.Services.AddSingleton<IStockRepository, InMemoryStockRepository>();
builder.Services.AddSingleton<StockService>();

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.MapInventoryEndpoints();

app.Run();
