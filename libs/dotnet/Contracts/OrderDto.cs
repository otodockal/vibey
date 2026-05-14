namespace Vibey.Contracts;

public record OrderLine(int ProductId, int Quantity);

public record OrderDto(
  int Id,
  string CustomerEmail,
  DateTime CreatedAt,
  IReadOnlyList<OrderLine> Lines,
  decimal Total
);

public record CreateOrderRequest(string CustomerEmail, IReadOnlyList<OrderLine> Lines);
