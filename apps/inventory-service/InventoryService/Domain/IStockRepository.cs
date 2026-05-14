namespace Vibey.Inventory.Domain;

public interface IStockRepository
{
  IReadOnlyList<StockItem> GetAll();
  StockItem? Get(int productId);
}
