using Vibey.Payments.Api;
using Vibey.Payments.Application;
using Vibey.Payments.Domain;
using Vibey.Payments.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddCors(o =>
  o.AddDefaultPolicy(p => p.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod())
);

builder.Services.AddSingleton<IPaymentRepository, InMemoryPaymentRepository>();
builder.Services.AddSingleton<PaymentService>();

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.MapPaymentEndpoints();

app.Run();
