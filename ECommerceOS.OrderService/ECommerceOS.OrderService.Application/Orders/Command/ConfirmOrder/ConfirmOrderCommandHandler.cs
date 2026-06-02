namespace ECommerceOS.OrderService.Application.Orders.Command.ConfirmOrder;

public class ConfirmOrderCommandHandler(IOrderRepository orderRepository) : ICommandHandler<ConfirmOrderCommand>
{
    public async Task<Result> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
    {
        var result = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
            .Bind(order => order.ConfirmOrder(request.ExpectedDeliveryDate))
            .TapAsync(async order => await orderRepository.UpdateAsync(order, cancellationToken));

        return result.Match(
            _ => Result.Success(),
            Result.Failure);
    }
}
