using Vibey.Contracts;
using Vibey.Products.Domain;

namespace Vibey.Products.Api;

internal static class ProductMapper
{
  public static ProductDto ToDto(this Product p) => new(p.Id, p.Name, p.Price, p.Stock);
}
