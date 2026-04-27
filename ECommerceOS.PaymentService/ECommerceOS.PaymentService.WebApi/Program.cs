using ECommerceOS.PaymentService.Application;
using ECommerceOS.PaymentService.Infrastructure;
using ECommerceOS.PaymentService.Infrastructure.Persistence;
using ECommerceOS.PaymentService.Presentation;
using ECommerceOS.PaymentService.Presentation.GrpcServices;
using ECommerceOS.PaymentService.Presentation.Payment;
using ECommerceOS.PaymentService.Presentation.StripePayment;
using ECommerceOS.PaymentService.Presentation.Transaction;
using ECommerceOS.ServiceDefaults;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddAuthentication(static options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme);

builder.Services.AddAuthorization();
builder.Services.AddProblemDetails();
builder.Services.AddGrpc(o =>
{
    o.EnableDetailedErrors = true;
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.AddInfrastructure();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapGrpcService<CheckoutService>();
app.MapTransactionsGroup();
app.MapPaymentsGroup();
app.MapStripeGroup();
app.MapDefaultEndpoints();

app.Run();
