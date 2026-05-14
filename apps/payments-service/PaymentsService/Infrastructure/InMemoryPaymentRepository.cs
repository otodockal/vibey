using Vibey.Payments.Domain;

namespace Vibey.Payments.Infrastructure;

public sealed class InMemoryPaymentRepository : IPaymentRepository
{
  private readonly List<Payment> _items = [];
  private readonly Lock _gate = new();
  private int _nextId;

  public IReadOnlyList<Payment> GetAll()
  {
    lock (_gate)
      return _items.ToArray();
  }

  public Payment Add(int orderId, decimal amount, PaymentStatus status)
  {
    var id = Interlocked.Increment(ref _nextId);
    var payment = new Payment(id, orderId, amount, status, DateTime.UtcNow);
    lock (_gate)
      _items.Add(payment);
    return payment;
  }
}
