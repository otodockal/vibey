using Vibey.Contracts;
using Vibey.Orders.Application;

namespace Vibey.Orders.Api;

internal static class OrderEndpoints
{
  public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
  {
    var orders = app.MapGroup("/api/orders").WithTags("Orders");

    orders
      .MapGet("/", (OrderService service) => Results.Ok(service.List().Select(o => o.ToDto())))
      .WithName("ListOrders");

    orders
      .MapPost(
        "/",
        async (CreateOrderRequest req, OrderService service) =>
        {
          var result = await service.CreateAsync(req.CustomerEmail, req.Lines.ToDomain());
          return result switch
          {
            CreateOrderResult.Ok ok => Results.Created(
              $"/api/orders/{ok.Order.Id}",
              ok.Order.ToDto()
            ),
            CreateOrderResult.UnknownProduct u => Results.BadRequest(
              new { error = $"Unknown product {u.ProductId}" }
            ),
            _ => Results.Problem(),
          };
        }
      )
      .WithName("CreateOrder");
  }
}
