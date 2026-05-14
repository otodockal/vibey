namespace Vibey.Orders.Domain;

public sealed record OrderLine(int ProductId, int Quantity);

public sealed class Order
{
  public int Id { get; }
  public string CustomerEmail { get; }
  public DateTime CreatedAt { get; }
  public IReadOnlyList<OrderLine> Lines { get; }
  public decimal Total { get; }

  public Order(
    int id,
    string customerEmail,
    DateTime createdAt,
    IReadOnlyList<OrderLine> lines,
    decimal total
  )
  {
    Id = id;
    CustomerEmail = customerEmail;
    CreatedAt = createdAt;
    Lines = lines;
    Total = total;
  }
}
