namespace ECommerceOS.OrderService.Application.Orders.Query.GetOrder;

public class GetOrderQueryHandler(IOrderRepository orderRepository)
    : IQueryHandler<GetOrderQuery, GetOrderQueryResponse>
{
    public async Task<Result<GetOrderQueryResponse>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId!, cancellationToken);

        return order.Match(
            success => Result<GetOrderQueryResponse>.Success(new GetOrderQueryResponse(success.Id)),
            Result<GetOrderQueryResponse>.Failure);
    }
}