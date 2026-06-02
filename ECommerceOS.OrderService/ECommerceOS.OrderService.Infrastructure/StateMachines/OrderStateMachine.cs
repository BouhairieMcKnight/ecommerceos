using ECommerceOS.Shared.Contracts.Messaging.Catalog;
using ECommerceOS.Shared.Contracts.Messaging.Order;
using ECommerceOS.Shared.Contracts.Messaging.Payment;
using MassTransit;
using MediatR;

namespace ECommerceOS.OrderService.Infrastructure.StateMachines;

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public OrderStateMachine()
    {
        Event(() => TransactionSubmitted, x =>
        {
            x.CorrelateById(context => context.Message.OrderId!.Value);
            x.SelectId(context => context.Message.OrderId!.Value);
        });
        Event(() => TransactionConfirmed, x => x.CorrelateById(context => context.Message.OrderId.Value));
        Event(() => TransactionCancelled, x => x.CorrelateById(context => context.Message.OrderId.Value));
        Event(() => ReserveSucceeded, x => x.CorrelateById(context => context.Message.OrderId.Value));
        Event(() => ReserveFailed, x => x.CorrelateById(context => context.Message.OrderId.Value));
        Event(() => SubmitOrderSucceeded, x => x.CorrelateById(context => context.Message.OrderId.Value));
        Event(() => SubmitOrderFailed, x => x.CorrelateById(context => context.Message.OrderId.Value));
        Event(() => ConfirmOrderSucceeded, x => x.CorrelateById(context => context.Message.OrderId.Value));
        Event(() => ConfirmOrderFailed, x => x.CorrelateById(context => context.Message.OrderId.Value));
        
        InstanceState(x => x.CurrentState);

        Initially(
            When(TransactionSubmitted)
                .Then(context =>
                {
                    context.Saga.OrderId = context.Message.OrderId!;
                    context.Saga.TransactionId = context.Message.TransactionId!;
                    context.Saga.TransactionDate = context.Message.CreatedAt;
                    context.Saga.CustomerId = context.Message.CustomerId!;
                    context.Saga.CheckoutDtos = context.Message.TransactionItems;
                    context.Saga.ShippingAddress = context.Message.Address;
                    context.Saga.Amount = Money.Create(
                        context.Message.TransactionItems.FirstOrDefault()!.Cost.CurrencyValue,
                        context.Message.TransactionItems.Sum(item => item.Cost.Amount))!;
                })
                .PublishAsync(context => context.Init<ReserveInventory>(new
                {
                    OrderId = context.Saga.OrderId,
                    CustomerId = context.Saga.CustomerId,
                    ReserveProducts = context.Saga.CheckoutDtos.Select(order => new ReserveProduct
                    {
                        ProductId = order.ProductId,
                        Quantity = order.Quantity
                    }).ToList()
                }))
                .TransitionTo(ReservingInventory)
        );

        During(ReservingInventory,
            When(ReserveSucceeded)
                .PublishAsync(context => context.Init<SubmitOrder>(new
                {
                    OrderId = context.Saga.OrderId,
                    CustomerId =  context.Saga.CustomerId,
                    OrderItems = context.Saga.CheckoutDtos,
                    TransactionId = context.Saga.TransactionId,
                    ShippingAddress = context.Saga.ShippingAddress
                }))
                .TransitionTo(SubmittingOrder),
            When(ReserveFailed)
                .Then(context => context.Saga.Status = nameof(Failed))
                .Produce(context => context.Init<InventoryReserveFailed>(new InventoryReserveFailed
                {
                    OrderId = context.Saga.OrderId,
                    TransactionId = context.Saga.TransactionId,
                    CreatedAt =  DateTime.UtcNow,
                    CartId =  context.Saga.CartId,
                    Version = 1
                }))
                .Finalize()
        );
        
        During(SubmittingOrder,
            When(SubmitOrderSucceeded)
                .TransitionTo(ProcessingTransaction),
            When(SubmitOrderFailed)
                .Then(context => context.Saga.Status = nameof(Failed))
                .Produce(context => context.Init<OrderFailed>(new OrderFailed
                {
                    TransactionId = context.Saga.TransactionId,
                    CreatedAt = DateTime.UtcNow,
                    Reason = context.Saga.Status,
                    OrderItems = context.Saga.CheckoutDtos,
                    CustomerId = context.Saga.CustomerId,
                    Version = 1
                }))
                .Finalize());
        
        During(ProcessingTransaction,
            When(TransactionConfirmed)
                .PublishAsync(context => context.Init<ConfirmOrder>(new
                {
                    OrderId = context.Saga.OrderId,
                    ExpectedDeliveryDate = DateTimeOffset.UtcNow.AddDays(3)
                }))
                .TransitionTo(ConfirmingOrder),
            When(TransactionCancelled)
                .Then(context => context.Saga.Status = nameof(TransactionCancelled))
                .PublishAsync(context => context.Init<CancelOrder>(new
                {
                    OrderId = context.Saga.OrderId,
                    CustomerId = context.Saga.CustomerId,
                    Reason = context.Saga.Status
                }))
                .Finalize()
        );
        
        During(ConfirmingOrder, 
            When(ConfirmOrderSucceeded)
                .Then(context =>
                {
                    context.Saga.ExpectedDeliveryDate = context.Message.ExpectedDeliveryDate;
                    context.Saga.Status = "Confirmed";
                })
                .Finalize(),
            When(ConfirmOrderFailed)
                .Then(context => context.Saga.Status = nameof(ConfirmOrderFailed))
                .PublishAsync(context => context.Init<CancelOrder>(new
                {
                    OrderId = context.Saga.OrderId,
                    CustomerId = context.Saga.CustomerId,
                    Reason = nameof(ConfirmOrderFailed)
                    
                }))
                .Finalize());
        
        SetCompletedWhenFinalized(); 
    }
    
    public State? ProcessingTransaction { get; private set; }
    public State? ReservingInventory { get; private set; }
    public State? Failed { get; private set; }
    public State? SubmittingOrder { get; private set; }
    public State? ConfirmingOrder { get; private set; }
    

    public Event<TransactionSubmitted>? TransactionSubmitted { get; private set; }
    public Event<TransactionCancelled>? TransactionCancelled { get; private set; }
    public Event<ReserveSucceeded>? ReserveSucceeded { get; private set; }
    public Event<ReserveFailed>? ReserveFailed { get; private set; }
    public Event<SubmitOrderSucceeded>? SubmitOrderSucceeded { get; private set; }
    public Event<SubmitOrderFailed>? SubmitOrderFailed { get; private set; }
    public Event<ConfirmOrderSucceeded>? ConfirmOrderSucceeded { get; private set; }
    public Event<ConfirmOrderFailed>? ConfirmOrderFailed { get; private set; }
    public Event<TransactionConfirmed>? TransactionConfirmed { get; private set; }
}
