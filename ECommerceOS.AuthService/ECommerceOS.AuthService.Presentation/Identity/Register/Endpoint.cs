namespace ECommerceOS.AuthService.Api.Identity.Register;

public static class Endpoint
{
    private const string Route = "/register";
    
    public static RouteHandlerBuilder MapRegisterEndpoint(this RouteGroupBuilder groupBuilder)
    {
        return groupBuilder.MapPost(Route, HandleAsync);
    }

    private static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        [FromBody] RegisterCommand command,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(command, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : result.ToProblemDetails();
    }
}
