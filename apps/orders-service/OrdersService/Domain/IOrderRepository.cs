namespace Vibey.Orders.Domain;

public interface IOrderRepository
{
  IReadOnlyList<Order> GetAll();
  Order Add(string customerEmail, IReadOnlyList<OrderLine> lines, decimal total);
}
