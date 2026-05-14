namespace Vibey.Inventory.Domain;

public sealed class StockItem
{
  public int ProductId { get; }
  public int Available { get; private set; }
  public int Reserved { get; private set; }

  public StockItem(int productId, int available, int reserved = 0)
  {
    ProductId = productId;
    Available = available;
    Reserved = reserved;
  }

  public bool TryReserve(int quantity)
  {
    if (quantity <= 0 || quantity > Available)
      return false;
    Available -= quantity;
    Reserved += quantity;
    return true;
  }
}
