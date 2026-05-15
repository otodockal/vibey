# Backend service patterns (.NET 10 minimal API)

Each service under `apps/<name>-service/<Name>Service/` follows a DDD-lite layout. Reference implementation: [apps/orders-service/OrdersService/](apps/orders-service/OrdersService/).

## Layering

```
<Name>Service/
  Program.cs                          composition root
  Api/<Entity>Endpoints.cs            MapGroup + handlers (static ext methods)
  Api/<Entity>Mapper.cs               Domain ↔ DTO (static ext methods)
  Application/<Entity>Service.cs      use cases, result types
  Domain/<Entity>.cs                  sealed records/classes
  Domain/I<Entity>Repository.cs       interfaces only
  Infrastructure/InMemory*.cs         repo implementations
  Infrastructure/Http*.cs             outbound HTTP clients
```

Namespace pattern: `Vibey.<Bounded>.<Layer>` (e.g. `Vibey.Orders.Domain`). Keep layer boundaries: `Api` and `Application` may know `Domain`; `Domain` knows nothing else; `Infrastructure` implements `Domain` interfaces.

## Program.cs composition

Pattern (see [orders Program.cs](apps/orders-service/OrdersService/Program.cs)):

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddCors(o =>
  o.AddDefaultPolicy(p => p.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod())
);

// Inter-service HTTP — URL from config, never hardcoded.
builder.Services.AddHttpClient<IProductCatalog, HttpProductCatalog>(c =>
{
  var url = builder.Configuration["Services:Products"] ?? "http://localhost:5101";
  c.BaseAddress = new Uri(url);
});

builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
builder.Services.AddScoped<OrderService>();

var app = builder.Build();
app.UseCors();
if (app.Environment.IsDevelopment()) app.MapOpenApi();
app.MapOrderEndpoints();
app.Run();
```

Rules:
- `AddSingleton` for repositories (in-memory stores hold state).
- `AddScoped` for application services.
- Typed `AddHttpClient<TInterface, TImpl>` for every outbound call; `BaseAddress` from `Services:<Name>` config so it survives env rewrites.
- CORS only allows `http://localhost:4200` (Angular dev server).
- `MapOpenApi` only in Development.

## Endpoints

Endpoints live in `Api/<Entity>Endpoints.cs` as a static extension method on `IEndpointRouteBuilder`, grouped under `/api/<plural>`. See [OrderEndpoints.cs](apps/orders-service/OrdersService/Api/OrderEndpoints.cs):

```csharp
internal static class OrderEndpoints
{
  public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
  {
    var orders = app.MapGroup("/api/orders").WithTags("Orders");

    orders.MapGet("/", (OrderService service) =>
      Results.Ok(service.List().Select(o => o.ToDto()))
    ).WithName("ListOrders");

    orders.MapPost("/", async (CreateOrderRequest req, OrderService service) =>
    {
      var result = await service.CreateAsync(req.CustomerEmail, req.Lines.ToDomain());
      return result switch
      {
        CreateOrderResult.Ok ok => Results.Created($"/api/orders/{ok.Order.Id}", ok.Order.ToDto()),
        CreateOrderResult.UnknownProduct u => Results.BadRequest(new { error = $"Unknown product {u.ProductId}" }),
        _ => Results.Problem(),
      };
    }).WithName("CreateOrder");
  }
}
```

Rules:
- Endpoint group prefix is always `/api/<plural>` — the FE proxy depends on it.
- Use the request DTO type directly as a parameter (minimal API binds JSON automatically).
- Return DTOs, never domain types — convert via the mapper.
- Errors are modeled as result types (`abstract record` + sealed cases), pattern-matched in the endpoint to choose the right `Results.*`. Do not throw for expected failures.

## Application services & result types

Application services receive repository + outbound clients via primary constructors and return result types:

```csharp
public abstract record CreateOrderResult
{
  public sealed record Ok(Order Order) : CreateOrderResult;
  public sealed record UnknownProduct(int ProductId) : CreateOrderResult;
}

public sealed class OrderService(IOrderRepository repository, IProductCatalog catalog) { ... }
```

## Domain

- Records for immutable value-like types (`OrderLine`).
- Sealed classes when you want a constructor that enforces invariants (`Order`).
- Repository interfaces in `Domain/`; implementations in `Infrastructure/`.

## DTOs and mapping

- Cross-service DTOs go in [libs/dotnet/Contracts/](libs/dotnet/Contracts/) (`Vibey.Contracts` namespace). The service's `.csproj` references `Contracts.csproj`.
- Each service has a `Api/<Entity>Mapper.cs` with `ToDto` / `ToDomain` static extension methods.
- **Any change to a Contracts record must be mirrored in [libs/shared-ui/src/lib/models.ts](libs/shared-ui/src/lib/models.ts).**

## Adding a new microservice

1. Create `apps/<name>-service/<Name>Service/<Name>Service.csproj` referencing `Contracts.csproj`.
2. Scaffold the four folders (Api/Application/Domain/Infrastructure), `Program.cs`, and `Properties/launchSettings.json` with a free `51xx` port.
3. Add the csproj to [Backend.sln](Backend.sln).
4. Add a `/api/<name>` route in [apps/web/proxy.conf.json](apps/web/proxy.conf.json) pointing to the new port.
5. Generate the FE data-access lib: `pnpm nx g @nx/angular:lib libs/<name>-data-access --standalone`. See [references/frontend-feature.md](frontend-feature.md).
6. Add `pnpm start:<name>` and start/build entries in [package.json](package.json) scripts.

`@nx/dotnet` infers Nx targets from the `.csproj` — do not create a `project.json` unless overriding defaults.
