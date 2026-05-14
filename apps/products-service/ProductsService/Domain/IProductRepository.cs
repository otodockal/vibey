namespace Vibey.Products.Domain;

public interface IProductRepository
{
  IReadOnlyList<Product> GetAll();
  Product? Get(int id);
}
