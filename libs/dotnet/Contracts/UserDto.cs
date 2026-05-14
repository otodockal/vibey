namespace Vibey.Contracts;

public record UserDto(int Id, string Email, string Name, DateTime CreatedAt);

public record CreateUserRequest(string Email, string Name);
