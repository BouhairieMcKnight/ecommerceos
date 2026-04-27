using ECommerceOS.OrderService.Application.Orders.Command.SubmitOrder;
using ECommerceOS.Shared.Contracts.Messaging.Payment;

namespace ECommerceOS.OrderService.Application.Orders.EventHandlers;

public class TransactionCompletedEventHandler(IMediator mediator) : INotificationHandler<TransactionSubmitted>
{
    public async Task Handle(TransactionSubmitted notification, CancellationToken cancellationToken)
    {
        var command = new SubmitOrderCommand(
            notification.OrderId,
            notification.CustomerId,
            notification.TransactionId,
            notification.Address,
            notification.TransactionItems);

        await mediator.Send(command, cancellationToken);
    }
}
