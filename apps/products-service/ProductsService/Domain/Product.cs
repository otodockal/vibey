namespace Vibey.Products.Domain;

public sealed class Product
{
  public int Id { get; }
  public string Name { get; }
  public decimal Price { get; }
  public int Stock { get; private set; }

  public Product(int id, string name, decimal price, int stock)
  {
    Id = id;
    Name = name;
    Price = price;
    Stock = stock;
  }
}
