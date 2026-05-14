using Vibey.Inventory.Domain;

namespace Vibey.Inventory.Application;

public abstract record ReserveResult
{
  public sealed record Ok(StockItem Item) : ReserveResult;

  public sealed record InsufficientStock(StockItem Item) : ReserveResult;

  public sealed record UnknownProduct(int ProductId) : ReserveResult;
}

public sealed class StockService(IStockRepository repository)
{
  private readonly Lock _gate = new();

  public IReadOnlyList<StockItem> List() => repository.GetAll();

  public ReserveResult Reserve(int productId, int quantity)
  {
    lock (_gate)
    {
      var item = repository.Get(productId);
      if (item is null)
        return new ReserveResult.UnknownProduct(productId);

      return item.TryReserve(quantity)
        ? new ReserveResult.Ok(item)
        : new ReserveResult.InsufficientStock(item);
    }
  }
}
