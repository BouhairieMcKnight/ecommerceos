using ECommerceOS.CatalogService.Application.Products.Command.ReserveInventory;
using ECommerceOS.ReservationService;
using Grpc.Core;

namespace ECommerceOS.CatalogService.Presentation.GrpcServices;

public class InventoryReserveService(IMediator mediator) : Reserve.ReserveBase
{
    public override async Task<ReservationResponse> ReserveInventory(
        ReserveInventoryRequest request,
        ServerCallContext context)
    {
        var idempotencyId = new Guid(request.IdempotentId);
        var userId = new UserId(new Guid(request.UserId));
        
        var products = request.Items.ToDictionary(
            item => new ProductId(Guid.Parse(item.ProductId)),
            item => item.Quantity);

        var command = new ReserveInventoryCommand
        {
            ReserveProducts = products,
            IdempotentCommandId = idempotencyId,
            UserId = userId
        };
        
        var result = await mediator.Send(command);

        var response = new ReservationResponse
        {
            Error = result.IsSuccess ? "" : result.Error!.Description,
        };

        return response;
    }
}