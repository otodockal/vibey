using Vibey.Products.Application;

namespace Vibey.Products.Api;

internal static class ProductEndpoints
{
  public static void MapProductEndpoints(this IEndpointRouteBuilder app)
  {
    var products = app.MapGroup("/api/products").WithTags("Products");

    products
      .MapGet("/", (ProductService service) => Results.Ok(service.List().Select(p => p.ToDto())))
      .WithName("ListProducts");

    // Used internally by OrdersService to look up line prices when creating an order.
    products
      .MapGet(
        "/{id:int}",
        (int id, ProductService service) =>
          service.Get(id) is { } p ? Results.Ok(p.ToDto()) : Results.NotFound()
      )
      .WithName("GetProduct");
  }
}
