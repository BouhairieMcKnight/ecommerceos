namespace ECommerceOS.CatalogService.Application.Products.Command.ReserveInventory;

public class ReserveInventoryCommandHandler(ICatalogRepository catalogRepository)
    : ICommandHandler<ReserveInventoryCommand>
{
    public async Task<Result> Handle(ReserveInventoryCommand request, CancellationToken cancellationToken)
    {
        var result = await catalogRepository.ReserveInventoryAsync(request.ReserveProducts, cancellationToken);

        return result;
    }
}