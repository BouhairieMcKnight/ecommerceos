namespace ECommerceOS.OrderService.Application.Orders.Command.CancelOrder;

public class CancelOrderCommandHandler(IOrderRepository orderRepository) 
    : ICommandHandler<CancelOrderCommand>
{
    public async Task<Result> Handle(CancelOrderCommand request,
        CancellationToken cancellationToken)
    {
        var result = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
            .Bind(o => o.CancelOrder())
            .TapAsync(async o => await orderRepository.UpdateAsync(o, cancellationToken));

        return result.Match(
            success => Result.Success(),
            Result.Failure);
    }
}