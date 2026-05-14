namespace Vibey.Users.Domain;

public sealed class User
{
  public int Id { get; }
  public string Email { get; }
  public string Name { get; }
  public DateTime CreatedAt { get; }

  public User(int id, string email, string name, DateTime createdAt)
  {
    Id = id;
    Email = email;
    Name = name;
    CreatedAt = createdAt;
  }
}
