namespace ECommerceOS.OrderService.Application.Orders.Query.GetOrder;

public class GetOrderQueryValidator : AbstractValidator<GetOrderQuery>
{
    public GetOrderQueryValidator(IOrderRepository orderRepository)
    {
        RuleFor(query => query.UserId)
            .NotNull()
            .WithMessage("User cannot be null");
        
        RuleFor(query => query.OrderId)
            .NotNull()
            .WithMessage("Order Id cannot be null");
        
        RuleFor(q => q)
            .MustAsync(async (q, ct) => 
                await orderRepository.VerifyOrderAsync(q.OrderId!, q.UserId!,  ct))
            .When(c => c.UserId is not null && c.OrderId is not null)
            .WithMessage("Order not found");
    }
}