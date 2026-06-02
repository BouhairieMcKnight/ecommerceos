using ECommerceOS.AuthService.Api.Identity;
using ECommerceOS.AuthService.Application;
using ECommerceOS.AuthService.Infrastructure;
using ECommerceOS.AuthService.WebApi;
using ECommerceOS.ServiceDefaults;
using Schemes = ECommerceOS.AuthService.Api.Authentication.Schemes;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddApplication();
builder.AddInfrastructure();
builder.Services.AddAuthentication(static options =>
    {
        options.DefaultScheme = Schemes.DefaultScheme;
        options.DefaultAuthenticateScheme = Schemes.DefaultScheme;
        options.DefaultChallengeScheme = Schemes.DefaultScheme;
        options.DefaultSignInScheme = Schemes.ExternalCookieScheme;
    })
    .AddCookie(Schemes.ExternalCookieScheme, static options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
    })
    .AddJwtBearer(Schemes.DefaultScheme);


builder.Services.AddAuthorization();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
};

app.UseHttpsRedirection();
app.MapDefaultEndpoints();
app.MapAuthEndpoints();

app.Run();
