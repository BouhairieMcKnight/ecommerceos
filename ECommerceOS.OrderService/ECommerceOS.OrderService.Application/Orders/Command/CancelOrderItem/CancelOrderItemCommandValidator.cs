namespace ECommerceOS.OrderService.Application.Orders.Command.CancelOrderItem;

public class CancelOrderItemCommandValidator : AbstractValidator<CancelOrderItemCommand>
{
    public CancelOrderItemCommandValidator(IOrderRepository orderRepository)
    {
        RuleFor(command => command.UserId)
            .NotNull()
            .WithMessage("User cannot be null");
        
        RuleFor(c => c)
            .MustAsync(async (c, ct) => 
                await orderRepository.VerifyOrderAsync(c.OrderId, c.UserId!,  ct))
            .When(c => c.UserId is not null)
            .WithMessage("Order not found");
    }
}