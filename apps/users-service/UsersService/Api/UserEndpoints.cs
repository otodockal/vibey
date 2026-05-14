using Vibey.Contracts;
using Vibey.Users.Application;

namespace Vibey.Users.Api;

internal static class UserEndpoints
{
  public static void MapUserEndpoints(this IEndpointRouteBuilder app)
  {
    var users = app.MapGroup("/api/users").WithTags("Users");

    users
      .MapGet("/", (UserService service) => Results.Ok(service.List().Select(u => u.ToDto())))
      .WithName("ListUsers");

    users
      .MapPost(
        "/",
        (CreateUserRequest req, UserService service) =>
        {
          var result = service.Create(req.Email, req.Name);
          return result switch
          {
            CreateUserResult.Ok ok => Results.Created($"/api/users/{ok.User.Id}", ok.User.ToDto()),
            CreateUserResult.DuplicateEmail d => Results.Conflict(
              new { error = $"Email {d.Email} already exists" }
            ),
            _ => Results.Problem(),
          };
        }
      )
      .WithName("CreateUser");
  }
}
