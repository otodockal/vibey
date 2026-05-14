using Vibey.Contracts;
using Vibey.Payments.Domain;

namespace Vibey.Payments.Api;

internal static class PaymentMapper
{
  public static PaymentDto ToDto(this Payment p) =>
    new(p.Id, p.OrderId, p.Amount, p.Status.ToString(), p.CreatedAt);
}
