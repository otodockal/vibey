using Vibey.Products.Domain;

namespace Vibey.Products.Application;

public sealed class ProductService(IProductRepository repository)
{
  public IReadOnlyList<Product> List() => repository.GetAll();

  public Product? Get(int id) => repository.Get(id);
}
