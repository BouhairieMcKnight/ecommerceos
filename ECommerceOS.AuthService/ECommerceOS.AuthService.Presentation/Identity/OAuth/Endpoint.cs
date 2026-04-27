using ECommerceOS.AuthService.Api.Http;

namespace ECommerceOS.AuthService.Api.Identity.OAuth;

public static class Endpoint
{
    private const string Route = "/oauth/login-google";
    private const string Callback = "/oauth/google-response";

    public static RouteHandlerBuilder MapOauthEndpoint(this RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(Route, Handle);
        return groupBuilder.MapGet(Callback, HandleAsync);
    }


    private static IResult Handle(
        HttpContext httpContext,
        CancellationToken cancellationToken = default)
    {
        var properties = new AuthenticationProperties { RedirectUri = "/catalog" };
        return Results.Challenge(properties, [Schemes.GoogleOidcScheme]);
    }

    private static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default)
    {
        var newUser = httpContext.User.HasClaim(c => c is { Type: "new_user", Value: "y" });
        var email = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;
        var username = httpContext.User.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(username))
        {
            return Results.BadRequest("External provider did not return required user claims.");
        }

        if (newUser)
        {
            var command = new RegisterCommand(
                Email: email,
                Username: username,
                Password: null,
                External: true,
                Role: Role.Customer);
            var result = await sender.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                return result.ToProblemDetails();
            }
        }

        var loginCommand = new LoginOauthCommand(Email: email, Username: username);
        var loginResult = await sender.Send(loginCommand, cancellationToken);

        if (!loginResult.IsSuccess)
        {
            return loginResult.ToProblemDetails();
        }

        httpContext.SetRefreshToken(loginResult.Value!.RefreshToken);
        await httpContext.SignOutAsync(Schemes.ExternalCookieScheme);
        return Results.Ok(loginResult.Value.AccessToken);
    }
}
