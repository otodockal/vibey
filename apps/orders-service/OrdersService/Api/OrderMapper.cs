using Vibey.Contracts;
using DomainOrder = Vibey.Orders.Domain.Order;
using DomainOrderLine = Vibey.Orders.Domain.OrderLine;

namespace Vibey.Orders.Api;

internal static class OrderMapper
{
  public static OrderDto ToDto(this DomainOrder o) =>
    new(
      o.Id,
      o.CustomerEmail,
      o.CreatedAt,
      o.Lines.Select(l => new OrderLine(l.ProductId, l.Quantity)).ToArray(),
      o.Total
    );

  public static IReadOnlyList<DomainOrderLine> ToDomain(this IReadOnlyList<OrderLine> lines) =>
    lines.Select(l => new DomainOrderLine(l.ProductId, l.Quantity)).ToArray();
}
