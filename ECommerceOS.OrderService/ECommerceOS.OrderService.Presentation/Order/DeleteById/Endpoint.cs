namespace ECommerceOS.OrderService.Presentation.Order.DeleteById;

public static class Endpoint
{
    private const string Route = "/{id}";

    public static RouteHandlerBuilder MapDeleteByIdEndpoint(this RouteGroupBuilder groupBuilder)
    {
        return groupBuilder.MapDelete(Route, HandleAsync);
    }

    private static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        [FromRoute] string id,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default)
    {
        var orderId = new OrderId(Guid.Parse(id));
        var userId = httpContext.GetUserId();
        var command = new CancelOrderCommand(orderId, userId);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? Results.NoContent() : result.ToProblemDetails();
    }
}