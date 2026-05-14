using Vibey.Payments.Domain;

namespace Vibey.Payments.Application;

public abstract record CreatePaymentResult
{
  public sealed record Ok(Payment Payment) : CreatePaymentResult;

  public sealed record InvalidAmount(decimal Amount) : CreatePaymentResult;
}

public sealed class PaymentService(IPaymentRepository repository)
{
  public IReadOnlyList<Payment> List() => repository.GetAll();

  public CreatePaymentResult Create(int orderId, decimal amount)
  {
    if (amount <= 0m)
      return new CreatePaymentResult.InvalidAmount(amount);

    // Fake gateway: even-cent amounts capture, odd-cent amounts fail.
    var status = ((int)(amount * 100m)) % 2 == 0 ? PaymentStatus.Captured : PaymentStatus.Failed;

    return new CreatePaymentResult.Ok(repository.Add(orderId, amount, status));
  }
}
