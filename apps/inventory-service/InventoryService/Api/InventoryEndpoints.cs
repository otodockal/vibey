using Vibey.Contracts;
using Vibey.Inventory.Application;

namespace Vibey.Inventory.Api;

internal static class InventoryEndpoints
{
  public static void MapInventoryEndpoints(this IEndpointRouteBuilder app)
  {
    var inventory = app.MapGroup("/api/inventory").WithTags("Inventory");

    inventory
      .MapGet("/", (StockService service) => Results.Ok(service.List().Select(s => s.ToDto())))
      .WithName("ListStock");

    inventory
      .MapPost(
        "/reserve",
        (ReserveRequest req, StockService service) =>
        {
          var result = service.Reserve(req.ProductId, req.Quantity);
          return result switch
          {
            ReserveResult.Ok ok => Results.Ok(
              new ReserveResponse(ok.Item.ProductId, ok.Item.Available, ok.Item.Reserved, true)
            ),
            ReserveResult.InsufficientStock i => Results.Ok(
              new ReserveResponse(i.Item.ProductId, i.Item.Available, i.Item.Reserved, false)
            ),
            ReserveResult.UnknownProduct u => Results.NotFound(
              new { error = $"Unknown product {u.ProductId}" }
            ),
            _ => Results.Problem(),
          };
        }
      )
      .WithName("ReserveStock");
  }
}
