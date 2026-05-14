using Vibey.Contracts;
using Vibey.Inventory.Domain;

namespace Vibey.Inventory.Api;

internal static class StockMapper
{
  public static StockItemDto ToDto(this StockItem s) =>
    new(s.ProductId, s.Available, s.Reserved);
}
