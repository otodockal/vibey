namespace Vibey.Orders.Domain;

public sealed record ProductPrice(int Id, decimal Price);

public interface IProductCatalog
{
  Task<ProductPrice?> GetAsync(int productId);
}
