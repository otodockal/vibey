using Vibey.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddCors(o =>
  o.AddDefaultPolicy(p => p.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod())
);

// Talk to ProductsService over HTTP. URL comes from config so it can be
// rewritten by Aspire / Container Apps / k8s without code changes.
builder.Services.AddHttpClient<ProductsClient>(c =>
{
  var url = builder.Configuration["Services:Products"] ?? "http://localhost:5101";
  c.BaseAddress = new Uri(url);
});

builder.Services.AddSingleton<OrderStore>();

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

var orders = app.MapGroup("/api/orders").WithTags("Orders");

orders.MapGet("/", (OrderStore store) => Results.Ok(store.GetAll())).WithName("ListOrders");

orders
  .MapPost(
    "/",
    async (CreateOrderRequest req, OrderStore store, ProductsClient products) =>
    {
      // Look up each line's product to compute the total. In a real system
      // this would also validate stock and reserve it.
      decimal total = 0m;
      foreach (var line in req.Lines)
      {
        var product = await products.GetAsync(line.ProductId);
        if (product is null)
          return Results.BadRequest(new { error = $"Unknown product {line.ProductId}" });
        total += product.Price * line.Quantity;
      }

      var created = store.Create(req, total);
      return Results.Created($"/api/orders/{created.Id}", created);
    }
  )
  .WithName("CreateOrder");

app.Run();

internal sealed class ProductsClient(HttpClient http)
{
  public async Task<ProductDto?> GetAsync(int id)
  {
    var resp = await http.GetAsync($"/api/products/{id}");
    if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
      return null;
    resp.EnsureSuccessStatusCode();
    return await resp.Content.ReadFromJsonAsync<ProductDto>();
  }
}

internal sealed class OrderStore
{
  private readonly List<OrderDto> _items = [];
  private readonly Lock _gate = new();
  private int _nextId;

  public IReadOnlyList<OrderDto> GetAll()
  {
    lock (_gate)
      return _items.ToArray();
  }

  public OrderDto Create(CreateOrderRequest req, decimal total)
  {
    var id = Interlocked.Increment(ref _nextId);
    var o = new OrderDto(id, req.CustomerEmail, DateTime.UtcNow, req.Lines, total);
    lock (_gate)
      _items.Add(o);
    return o;
  }
}
