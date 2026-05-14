namespace Vibey.Payments.Domain;

public enum PaymentStatus
{
  Pending,
  Captured,
  Failed,
}

public sealed class Payment
{
  public int Id { get; }
  public int OrderId { get; }
  public decimal Amount { get; }
  public PaymentStatus Status { get; }
  public DateTime CreatedAt { get; }

  public Payment(int id, int orderId, decimal amount, PaymentStatus status, DateTime createdAt)
  {
    Id = id;
    OrderId = orderId;
    Amount = amount;
    Status = status;
    CreatedAt = createdAt;
  }
}
