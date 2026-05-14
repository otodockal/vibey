using Vibey.Orders.Domain;

namespace Vibey.Orders.Application;

public abstract record CreateOrderResult
{
  public sealed record Ok(Order Order) : CreateOrderResult;

  public sealed record UnknownProduct(int ProductId) : CreateOrderResult;
}

public sealed class OrderService(IOrderRepository repository, IProductCatalog catalog)
{
  public IReadOnlyList<Order> List() => repository.GetAll();

  public async Task<CreateOrderResult> CreateAsync(
    string customerEmail,
    IReadOnlyList<OrderLine> lines
  )
  {
    // Look up each line's product to compute the total. In a real system
    // this would also validate stock and reserve it.
    decimal total = 0m;
    foreach (var line in lines)
    {
      var product = await catalog.GetAsync(line.ProductId);
      if (product is null)
        return new CreateOrderResult.UnknownProduct(line.ProductId);
      total += product.Price * line.Quantity;
    }

    return new CreateOrderResult.Ok(repository.Add(customerEmail, lines, total));
  }
}
