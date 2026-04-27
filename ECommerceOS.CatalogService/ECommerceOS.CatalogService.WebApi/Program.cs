using ECommerceOS.CatalogService.Application;
using ECommerceOS.CatalogService.Infrastructure;
using ECommerceOS.CatalogService.Infrastructure.Persistence.Data;
using ECommerceOS.CatalogService.Presentation;
using ECommerceOS.CatalogService.Presentation.Categories;
using ECommerceOS.CatalogService.Presentation.Carts;
using ECommerceOS.CatalogService.Presentation.GrpcServices;
using ECommerceOS.CatalogService.Presentation.Product;
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
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.AddInfrastructure();
builder.Services.AddGrpc();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.UseHttpsRedirection();
app.MapGrpcService<InventoryReserveService>();
app.MapProductsGroup();
app.MapCartGroup();
app.MapCategoriesGroup();
app.MapDefaultEndpoints();

app.Run();
