namespace ECommerceOS.OrderService.Application.Orders.Query.GetPaginatedOrders;

public class GetPaginatedOrdersQueryValidator : AbstractValidator<GetPaginatedOrdersQuery>
{
    public GetPaginatedOrdersQueryValidator(IOrderRepository orderRepository)
    {
        RuleFor(q => q.UserId)
            .NotEmpty()
            .NotNull()
            .WithMessage("Valid User Id is required");
        
        RuleFor(q => q.UserId)
            .MustAsync(async (id, ct) =>
                await orderRepository.VerifyUserOrderAsync(id!, ct))
            .When(id => id is not null)
            .WithMessage("No orders found under User Id");
    }
}