using ECommerceOS.OrderService.Application.Orders.Command.CancelOrder;
using ECommerceOS.OrderService.Infrastructure.StateMachines;
using MassTransit;
using MediatR;

namespace ECommerceOS.OrderService.Infrastructure.Consumers;

public class ProcessCancelOrderConsumer(IMediator mediator) : IConsumer<CancelOrder>
{
    public async Task Consume(ConsumeContext<CancelOrder> context)
    {
        var command = new CancelOrderCommand(context.Message.OrderId, context.Message.CustomerId);
        await mediator.Send(command, context.CancellationToken);
    }
}
