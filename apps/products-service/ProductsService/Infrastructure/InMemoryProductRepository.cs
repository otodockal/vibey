using Vibey.Products.Domain;

namespace Vibey.Products.Infrastructure;

public sealed class InMemoryProductRepository : IProductRepository
{
  private readonly Product[] _items =
  [
    new(1, "Espresso Blend", 14.90m, 25),
    new(2, "Colombian Single Origin", 18.50m, 100),
    new(3, "Decaf House Roast", 16.00m, 60),
  ];

  public IReadOnlyList<Product> GetAll() => _items;

  public Product? Get(int id) => _items.FirstOrDefault(p => p.Id == id);
}
