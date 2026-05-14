using Vibey.Inventory.Domain;

namespace Vibey.Inventory.Infrastructure;

public sealed class InMemoryStockRepository : IStockRepository
{
  // Seeded to mirror products-service product IDs.
  private readonly StockItem[] _items = [new(1, 25), new(2, 100), new(3, 60)];

  public IReadOnlyList<StockItem> GetAll() => _items;

  public StockItem? Get(int productId) => _items.FirstOrDefault(s => s.ProductId == productId);
}
