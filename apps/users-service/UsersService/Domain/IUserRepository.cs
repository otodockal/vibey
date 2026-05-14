namespace Vibey.Users.Domain;

public interface IUserRepository
{
  IReadOnlyList<User> GetAll();
  User? FindByEmail(string email);
  User Add(string email, string name);
}
