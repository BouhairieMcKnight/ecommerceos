using ECommerceOS.OrderService.Application;
using ECommerceOS.OrderService.Infrastructure;
using ECommerceOS.OrderService.Infrastructure.Persistence;
using ECommerceOS.OrderService.Presentation;
using ECommerceOS.OrderService.Presentation.Order;
using ECommerceOS.ServiceDefaults;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

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
builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.AddInfrastructure();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapOrderGroupEndpoints();
app.MapDefaultEndpoints();

app.Run();
