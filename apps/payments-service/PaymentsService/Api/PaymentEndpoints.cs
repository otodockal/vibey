using Vibey.Contracts;
using Vibey.Payments.Application;

namespace Vibey.Payments.Api;

internal static class PaymentEndpoints
{
  public static void MapPaymentEndpoints(this IEndpointRouteBuilder app)
  {
    var payments = app.MapGroup("/api/payments").WithTags("Payments");

    payments
      .MapGet("/", (PaymentService service) => Results.Ok(service.List().Select(p => p.ToDto())))
      .WithName("ListPayments");

    payments
      .MapPost(
        "/",
        (CreatePaymentRequest req, PaymentService service) =>
        {
          var result = service.Create(req.OrderId, req.Amount);
          return result switch
          {
            CreatePaymentResult.Ok ok => Results.Created(
              $"/api/payments/{ok.Payment.Id}",
              ok.Payment.ToDto()
            ),
            CreatePaymentResult.InvalidAmount i => Results.BadRequest(
              new { error = $"Invalid amount {i.Amount}" }
            ),
            _ => Results.Problem(),
          };
        }
      )
      .WithName("CreatePayment");
  }
}
