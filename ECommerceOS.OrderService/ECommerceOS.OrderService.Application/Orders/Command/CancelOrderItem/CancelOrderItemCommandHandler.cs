namespace ECommerceOS.OrderService.Application.Orders.Command.CancelOrderItem;

public class CancelOrderItemCommandHandler(IOrderRepository orderRepository) 
    : ICommandHandler<CancelOrderItemCommand>
{
    public async Task<Result> Handle(CancelOrderItemCommand request, CancellationToken cancellationToken)
    {
        var result = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
            .Bind(o => o.RemoveOrderItem(request.OrderItemId))
            .TapAsync(o => orderRepository.UpdateAsync(o, cancellationToken));

        return result.Match(
            success => Result.Success(),
            Result.Failure);
    }
}
