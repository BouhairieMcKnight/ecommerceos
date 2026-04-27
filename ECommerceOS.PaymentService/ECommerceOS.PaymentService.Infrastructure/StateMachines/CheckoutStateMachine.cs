using ECommerceOS.PaymentService.Application.Transactions.Command.CreateTransaction;
using ECommerceOS.Shared.Contracts.Messaging.Order;
using ECommerceOS.Shared.Contracts.Messaging.Payment;
using MassTransit;
using Event = MassTransit.Event;

namespace ECommerceOS.PaymentService.Infrastructure.StateMachines;

public class CheckoutStateMachine : MassTransitStateMachine<CheckoutState>
{
    public CheckoutStateMachine()
    {
        Event(() => PaymentIntentCompleted, x => x.CorrelateById(context => context.Message.TransactionId.Value));
        Event(() => PaymentIntentCapturable, x => x.CorrelateById(context => context.Message.TransactionId.Value));
        Event(() => OrderSubmitted, x => x.CorrelateById(context => context.Message.TransactionId.Value));
        Event(() => OrderCancelled, x => x.CorrelateById(context => context.Message.TransactionId.Value));
        Event(() => OrderFailed, x => x.CorrelateById(context => context.Message.TransactionId.Value));
        Event(() => TransactionFailed, x => x.CorrelateById(context => context.Message.TransactionId.Value));

        InstanceState(x => x.CurrentState);

        Initially(
                When(OrderSubmitted)
                    .Then(context =>
                    {
                        context.Saga.TransactionId = context.Message.TransactionId;
                        context.Saga.CustomerId = context.Message.CustomerId;
                        context.Saga.OrderId = context.Message.OrderId;
                        context.Saga.TransactionLineItems = context.Message.OrderItems;
                        context.Saga.TotalAmount = context.Message.OrderItems
                            .Sum(item => (long)item.Cost.Amount);
                    }).TransitionTo(ProcessingOrder),
                When(OrderFailed)
                    .Then(context =>
                    {
                        context.Saga.TransactionId = context.Message.TransactionId;
                        context.Saga.CustomerId = context.Message.CustomerId;
                    })
                    .PublishAsync(context => context.Init<CancelTransaction>(new CancelTransaction
                    {
                        TransactionId = context.Saga.TransactionId,
                        CustomerId = context.Saga.CustomerId,
                        Reason = context.Message.Reason
                    })).Finalize(),
            When(PaymentIntentCapturable)
                .Then(context =>
                {
                    context.Saga.CapturableAmount = context.Message.CapturableAmount;
                    context.Saga.Currency = context.Message.Currency;
                }).TransitionTo(ProcessingOrder));

        CompositeEvent(() => OrderReady, x => x.ReadyEventStatus, OrderSubmitted, PaymentIntentCapturable);

        During(ProcessingOrder,
            When(OrderReady)
                .IfElse(
                    context => context.Saga.ValidateCapturable,
                    then => then
                        .PublishAsync(x => x.Init<CapturePayment>(new CapturePayment
                        {
                            IdempotencyKey = x.Saga.IdempotencyKey ?? Guid.NewGuid(),
                            TransactionId = x.Saga.TransactionId,
                            OrderId = x.Saga.OrderId,
                            CustomerId = x.Saga.CustomerId,
                            Amount = x.Saga.TotalAmount,
                            Currency = x.Saga.Currency
                        }))
                        .TransitionTo(CapturingPayment),
                    @else => @else
                        .PublishAsync(context => context.Init<CancelTransaction>(new CancelTransaction
                        {
                            
                            TransactionId = context.Saga.TransactionId,
                            CustomerId = context.Saga.CustomerId,
                            Reason = "Insufficient Funds"
                        }))
                        .Finalize()));
        
        SetCompletedWhenFinalized();
    }
    
    public State? ProcessingOrder { get; private set; }
    public State? CapturingPayment { get; private set; }
    
    public Event? OrderReady { get; private set; }
    public Event<PaymentIntentCompleted>? PaymentIntentCompleted { get; private set; }
    public Event<PaymentIntentCapturable>? PaymentIntentCapturable { get; private set; }
    public Event<OrderSubmitted>? OrderSubmitted { get; private set; }
    public Event<OrderCancelled>? OrderCancelled { get; private set; }
    public Event<OrderFailed>? OrderFailed { get; private set; }
    public Event<TransactionFailed>? TransactionFailed { get; private set; }
}
