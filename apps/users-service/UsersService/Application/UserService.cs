using Vibey.Users.Domain;

namespace Vibey.Users.Application;

public abstract record CreateUserResult
{
  public sealed record Ok(User User) : CreateUserResult;

  public sealed record DuplicateEmail(string Email) : CreateUserResult;
}

public sealed class UserService(IUserRepository repository)
{
  public IReadOnlyList<User> List() => repository.GetAll();

  public CreateUserResult Create(string email, string name)
  {
    if (repository.FindByEmail(email) is not null)
      return new CreateUserResult.DuplicateEmail(email);

    return new CreateUserResult.Ok(repository.Add(email, name));
  }
}
