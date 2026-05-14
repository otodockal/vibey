using Vibey.Contracts;
using Vibey.Orders.Domain;

namespace Vibey.Orders.Infrastructure;

public sealed class HttpProductCatalog(HttpClient http) : IProductCatalog
{
  public async Task<ProductPrice?> GetAsync(int productId)
  {
    var resp = await http.GetAsync($"/api/products/{productId}");
    if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
      return null;
    resp.EnsureSuccessStatusCode();
    var dto = await resp.Content.ReadFromJsonAsync<ProductDto>();
    return dto is null ? null : new ProductPrice(dto.Id, dto.Price);
  }
}
