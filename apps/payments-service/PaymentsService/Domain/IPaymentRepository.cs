namespace Vibey.Payments.Domain;

public interface IPaymentRepository
{
  IReadOnlyList<Payment> GetAll();
  Payment Add(int orderId, decimal amount, PaymentStatus status);
}
