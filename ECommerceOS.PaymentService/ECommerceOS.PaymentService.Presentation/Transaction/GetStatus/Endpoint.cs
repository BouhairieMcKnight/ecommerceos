using ECommerceOS.PaymentService.Presentation.Http;

namespace ECommerceOS.PaymentService.Presentation.Transaction.GetStatus;

public static class Endpoint
{
    private const string Route = "session-status";

    public static RouteHandlerBuilder MapGetStatusEndpoint(this RouteGroupBuilder groupBuilder)
    {
        return groupBuilder.MapGet(Route, HandleAsync);
    }
    
    
    private static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        [FromQuery(Name = "session_id")] string sessionId,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default)
    {
        var userId = httpContext.GetUserId();
        var query = new GetTransactionStatusQuery(userId, sessionId);
        var result = await sender.Send(query, cancellationToken);
        
        return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
    }
}
