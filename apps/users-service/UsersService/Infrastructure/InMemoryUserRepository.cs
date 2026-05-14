using Vibey.Users.Domain;

namespace Vibey.Users.Infrastructure;

public sealed class InMemoryUserRepository : IUserRepository
{
  private readonly List<User> _items =
  [
    new(1, "ada@example.com", "Ada Lovelace", DateTime.UtcNow),
    new(2, "alan@example.com", "Alan Turing", DateTime.UtcNow),
  ];
  private readonly Lock _gate = new();
  private int _nextId = 2;

  public IReadOnlyList<User> GetAll()
  {
    lock (_gate)
      return _items.ToArray();
  }

  public User? FindByEmail(string email)
  {
    lock (_gate)
      return _items.FirstOrDefault(u =>
        string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase)
      );
  }

  public User Add(string email, string name)
  {
    var id = Interlocked.Increment(ref _nextId);
    var user = new User(id, email, name, DateTime.UtcNow);
    lock (_gate)
      _items.Add(user);
    return user;
  }
}
