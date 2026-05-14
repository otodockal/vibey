using Vibey.Contracts;
using Vibey.Users.Domain;

namespace Vibey.Users.Api;

internal static class UserMapper
{
  public static UserDto ToDto(this User u) => new(u.Id, u.Email, u.Name, u.CreatedAt);
}
