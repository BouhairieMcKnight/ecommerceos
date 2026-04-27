namespace ECommerceOS.OrderService.Application.Orders.Command.SubmitOrder;

public class SubmitOrderCommandHandler(IOrderRepository orderRepository) : ICommandHandler<SubmitOrderCommand>
{
    public async Task<Result> Handle(SubmitOrderCommand request, CancellationToken cancellationToken)
    {
        var creation = Order.Create(
            request.ShippingAddress,
            request.CustomerId,
            request.TransactionId,
            OrderStatus.Pending.ToString(),
            request.OrderId);

        if (!creation.IsSuccess)
        {
            return Result.Failure(creation.Error!);
        }

        var order = creation.Value!;
        foreach (var item in request.OrderItems)
        {
            var addItem = order.AddOrderItem(
                item.ProductId,
                item.Quantity,
                item.Cost,
                item.SellerId,
                item.ImageUrl);

            if (!addItem.IsSuccess)
            {
                return Result.Failure(addItem.Error!);
            }
        }

        await orderRepository.AddAsync(order, cancellationToken);
        return Result.Success();
    }
}
