namespace Vibey.Contracts;

public record PaymentDto(int Id, int OrderId, decimal Amount, string Status, DateTime CreatedAt);

public record CreatePaymentRequest(int OrderId, decimal Amount);
