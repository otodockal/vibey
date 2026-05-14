using Vibey.Products.Api;
using Vibey.Products.Application;
using Vibey.Products.Domain;
using Vibey.Products.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddCors(o =>
  o.AddDefaultPolicy(p => p.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod())
);

builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
builder.Services.AddSingleton<ProductService>();

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.MapProductEndpoints();

app.Run();
