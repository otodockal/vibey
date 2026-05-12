using Vibey.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddCors(o =>
  o.AddDefaultPolicy(p => p.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod())
);

// Trivial in-memory store. Replace with EF Core / Dapper / whatever in a real service.
builder.Services.AddSingleton<ProductStore>();

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

var products = app.MapGroup("/api/products").WithTags("Products");

products.MapGet("/", (ProductStore store) => Results.Ok(store.GetAll())).WithName("ListProducts");

// Used internally by OrdersService to look up line prices when creating an order.
products
  .MapGet(
    "/{id:int}",
    (int id, ProductStore store) => store.Get(id) is { } p ? Results.Ok(p) : Results.NotFound()
  )
  .WithName("GetProduct");

app.Run();

internal sealed class ProductStore
{
  private readonly ProductDto[] _items =
  [
    new(1, "Laser Goggles", 39.90m, 25),
    new(2, "PicoWay Cleanser", 14.50m, 100),
    new(3, "SPF 50 Sunscreen", 18.00m, 60),
  ];

  public IReadOnlyList<ProductDto> GetAll() => _items;

  public ProductDto? Get(int id) => _items.FirstOrDefault(p => p.Id == id);
}
