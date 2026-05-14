using Vibey.Orders.Api;
using Vibey.Orders.Application;
using Vibey.Orders.Domain;
using Vibey.Orders.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddCors(o =>
  o.AddDefaultPolicy(p => p.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod())
);

// Talk to ProductsService over HTTP. URL comes from config so it can be
// rewritten by Aspire / Container Apps / k8s without code changes.
builder.Services.AddHttpClient<IProductCatalog, HttpProductCatalog>(c =>
{
  var url = builder.Configuration["Services:Products"] ?? "http://localhost:5101";
  c.BaseAddress = new Uri(url);
});

builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
builder.Services.AddScoped<OrderService>();

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.MapOrderEndpoints();

app.Run();
