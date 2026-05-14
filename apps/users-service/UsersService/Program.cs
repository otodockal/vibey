using Vibey.Users.Api;
using Vibey.Users.Application;
using Vibey.Users.Domain;
using Vibey.Users.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddCors(o =>
  o.AddDefaultPolicy(p => p.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod())
);

builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<UserService>();

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.MapUserEndpoints();

app.Run();
