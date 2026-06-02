using ECommerceOS.PaymentService.Application.Transactions.Command.CancelTransaction;
using ECommerceOS.PaymentService.Application.Transactions.Command.CreateTransaction;
using ECommerceOS.PaymentService.Application.Transactions.EventHandlers;
using ECommerceOS.PaymentService.Infrastructure.StateMachines;
using ECommerceOS.Shared.Contracts.Messaging.Payment;
using MassTransit;

namespace ECommerceOS.PaymentService.Infrastructure.Consumers;

public class ProcessCancelTransactionConsumer(IMediator mediator) : IConsumer<CancelTransaction>
{
    public async Task Consume(ConsumeContext<CancelTransaction> context)
    {
        var command = new CancelTransactionCommand
        {
            TransactionId = context.Message.TransactionId,
            CustomerId = context.Message.CustomerId,
            Reason = context.Message.Reason
        };

        var result = await mediator.Send(command, context.CancellationToken);

        if (result.IsSuccess)
        {
            await context.Publish<TransactionSuccessfullyCancelled>(new TransactionSuccessfullyCancelled
            {
                TransactionId = context.Message.TransactionId,
                Status = result.Value!
            });
        }
            
        await context.Publish<TransactionFailedCancel>(new TransactionFailedCancel
        {
            TransactionId = context.Message.TransactionId,
            Error = result.Error!.Description
        });

        await context.ConsumeCompleted;
    }
}