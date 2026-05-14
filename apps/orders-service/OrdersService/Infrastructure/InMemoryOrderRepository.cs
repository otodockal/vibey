using Vibey.Orders.Domain;

namespace Vibey.Orders.Infrastructure;

public sealed class InMemoryOrderRepository : IOrderRepository
{
  private readonly List<Order> _items = [];
  private readonly Lock _gate = new();
  private int _nextId;

  public IReadOnlyList<Order> GetAll()
  {
    lock (_gate)
      return _items.ToArray();
  }

  public Order Add(string customerEmail, IReadOnlyList<OrderLine> lines, decimal total)
  {
    var id = Interlocked.Increment(ref _nextId);
    var order = new Order(id, customerEmail, DateTime.UtcNow, lines, total);
    lock (_gate)
      _items.Add(order);
    return order;
  }
}
