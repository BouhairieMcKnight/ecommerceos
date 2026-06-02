using Avro;
using ECommerceOS.PaymentService.Application.Transactions.Command.CreateTransaction;
using ECommerceOS.PaymentService.Infrastructure.Consumers;
using ECommerceOS.PaymentService.Infrastructure.StateMachines;
using ECommerceOS.Shared.Contracts.Interfaces;
using ECommerceOS.Shared.Contracts.Messaging.Payment;
using ECommerceOS.Shared.DTOs;
using MassTransit;
using Address = ECommerceOS.Shared.ValueObjects.Address;
using Event = Stripe.Event;

namespace ECommerceOS.PaymentService.Infrastructure.External.StripeGateway;

public class StripeProcessor(
    SessionService sessionService,
    ILogger<StripeProcessor> logger,
    PaymentIntentService paymentIntentService,
    PaymentDbContext dbContext,
    IPublishEndpoint publishEndpoint,
    IProducer<string, IIntegrationEvent> producer,
    IMediator mediator)
{
    public async Task HandleStripeEventAsync(Event stripeEvent, CancellationToken cancellationToken = default)
    {
        await (stripeEvent.Type switch
        {
            EventTypes.SetupIntentSucceeded =>
                ProcessStripeAccountSetupAsync(stripeEvent, cancellationToken),
            EventTypes.CheckoutSessionCompleted =>
                ProcessCheckoutCompletedAsync(stripeEvent, cancellationToken),
            EventTypes.PaymentIntentAmountCapturableUpdated =>
                ProcessPaymentCapturableAsync(stripeEvent, cancellationToken),
            EventTypes.PaymentIntentSucceeded =>
                ProcessPaymentCapturedAsync(stripeEvent, cancellationToken),
            _ => Task.CompletedTask
        });
    }

    private async Task ProcessPaymentCapturableAsync(Event stripeEvent, CancellationToken cancellationToken = default)
    {
        if (stripeEvent.Data.Object is not PaymentIntent paymentIntent)
        {
            logger.LogWarning("Stripe payment intent payload was missing.");
            return;
        }

        if (!TryGetTransactionId(paymentIntent.Metadata ?? new Dictionary<string, string>(), out var transactionId))
        {
            logger.LogWarning("Stripe payment intent capturable event missing TransactionId metadata.");
            return;
        }

        var message = new PaymentIntentCapturable
        {
            TransactionId = transactionId,
            CapturableAmount = paymentIntent.AmountCapturable,
            Currency = paymentIntent.Currency
        };

        await publishEndpoint.Publish(message, cancellationToken);
    }

    private async Task ProcessPaymentCapturedAsync(Event stripeEvent, CancellationToken cancellationToken = default)
    {
        if (stripeEvent.Data.Object is not PaymentIntent paymentIntent)
        {
            logger.LogWarning("Stripe payment intent payload was missing.");
            return;
        }

        if (!TryGetTransactionId(paymentIntent.Metadata ?? new Dictionary<string, string>(), out var transactionId))
        {
            logger.LogWarning("Stripe payment intent success event missing TransactionId metadata.");
            return;
        }

        await publishEndpoint.Publish(new PaymentIntentCapturable
        {
            Currency = paymentIntent.Currency,
            TransactionId = transactionId,
            CapturableAmount = paymentIntent.AmountCapturable,
        }, cancellationToken);
    }

    private async Task ProcessStripeAccountSetupAsync(Event stripeEvent, CancellationToken cancellationToken = default)
    {
        if (stripeEvent.Data.Object is not SetupIntent setup)
        {
            logger.LogWarning("Stripe setup intent payload was missing.");
            return;
        }

        if (!TryGetUserId(setup.Metadata ?? new Dictionary<string, string>(), out var userId))
        {
            logger.LogWarning("Stripe setup intent event missing UserId metadata.");
        }

        await publishEndpoint.Publish<SetupStripeAccount>(new
        {
            UserId = userId,
            AccountId = setup.Customer,
            CreatedAt = DateTime.UtcNow
            
        }, cancellationToken);


    }

    private async Task ProcessCheckoutCompletedAsync(Event stripeEvent, CancellationToken cancellationToken = default)
    {
        if (stripeEvent.Data.Object is not Session checkout)
        {
            logger.LogWarning("Stripe checkout session payload was missing.");
            return;
        }

        var sessionOptions = new SessionGetOptions
        {
            Expand =
            [
                "line_items",
                "line_items.data.price.product",
                "payment_intent",
                "payment_intent.payment_method"
            ]
        };

        var session = await sessionService.GetAsync(checkout.Id, sessionOptions, cancellationToken: cancellationToken);
        var metadata = session.Metadata ?? new Dictionary<string, string>();

        if (!TryGetUserId(metadata, out var customerId))
        {
            logger.LogWarning("Stripe checkout session missing AccountId metadata.");
            return;
        }

        if (!TryGetTransactionId(metadata, out var transactionId))
        {
            logger.LogWarning("Stripe checkout session missing TransactionId metadata.");
            return;
        }

        var paymentMethodNumber = session.PaymentIntent?.PaymentMethod?.Id;
        if (string.IsNullOrWhiteSpace(paymentMethodNumber))
        {
            var paymentIntentId = session.PaymentIntentId;
            if (!string.IsNullOrWhiteSpace(paymentIntentId))
            {
                var paymentIntent = await paymentIntentService.GetAsync(paymentIntentId, cancellationToken: cancellationToken);
                paymentMethodNumber = paymentIntent.PaymentMethodId;
            }
        }

        if (string.IsNullOrWhiteSpace(paymentMethodNumber))
        {
            logger.LogWarning("Stripe checkout session {SessionId} is missing payment method.", session.Id);
            return;
        }

        var customerPaymentId = await dbContext.Set<PaymentMetadata>()
            .AsNoTracking()
            .Where(p => p.Id == paymentMethodNumber)
            .Select(p => p.PaymentId)
            .FirstOrDefaultAsync(cancellationToken);

        if (customerPaymentId is null)
        {
            logger.LogWarning("Payment number {PaymentNumber} could not be resolved.", paymentMethodNumber);
            return;
        }

        var items = MapLineItems(session);
        if (items.Count == 0)
        {
            logger.LogWarning("Stripe checkout session {SessionId} had no mappable line items.", session.Id);
            return;
        }

        var shipping = session.CustomerDetails?.Address;
        if (shipping is null)
        {
            logger.LogWarning("Stripe checkout session {SessionId} did not contain a shipping address.", session.Id);
            return;
        }

        var status = session.PaymentStatus switch
        {
            "paid" => nameof(TransactionStatus.Confirmed),
            "unpaid" => nameof(TransactionStatus.Pending),
            "no_payment_required" => nameof(TransactionStatus.Confirmed),
            _ => nameof(TransactionStatus.Pending)
        };

        var result = await mediator.Send(new CreateTransactionCommand
        {
            CustomerId = customerId,
            CustomerPaymentId = customerPaymentId,
            ShippingAddress = Address.Create(
                $"{shipping.Line1}," +
                $"{shipping.City}," +
                $"{shipping.State}," +
                $"{shipping.Country}," +
                $"{shipping.PostalCode}"),
            TransactionLineItems = items.Select(item => new TransactionLineItem
            {
                ProductId = item.ProductId,
                SellerId = item.SellerId,
                Cost = item.Cost
            }),
            Status = status,
            TransactionId = transactionId
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            logger.LogWarning("Stripe checkout session {SessionId} failed.", session.Id);
            var canceledPayment = await paymentIntentService.CancelAsync(
                session.PaymentIntentId, cancellationToken: cancellationToken);

            var @event = new TransactionFailed
            {
                TransactionId = transactionId,
                CreatedAt = DateTime.UtcNow,
                CustomerId = customerId,
                Reason = result.Error!.Description,
            };

            var message = new Message<string, IIntegrationEvent>
            {
                Key = "payment-canceled",
                Value = @event
            };
            
            await producer.ProduceAsync("payment-event", message, cancellationToken);
        }
    }

    private static List<CheckoutDto> MapLineItems(Session session)
    {
        var items = new List<CheckoutDto>();

        if (session.LineItems is null)
        {
            return items;
        }

        foreach (var lineItem in session.LineItems)
        {
            var productMetadata = lineItem.Price?.Product?.Metadata;
            if (productMetadata is null)
            {
                continue;
            }

            if (!productMetadata.TryGetValue("ProductId", out var productIdValue) ||
                !Guid.TryParse(productIdValue, out var productId))
            {
                continue;
            }

            if (!productMetadata.TryGetValue("SellerId", out var sellerIdValue) ||
                !Guid.TryParse(sellerIdValue, out var sellerId))
            {
                continue;
            }

            var price = Convert.ToDecimal(lineItem.AmountTotal);
            var currency = lineItem.Currency ?? "usd";
            var quantity = Convert.ToInt32(lineItem.Quantity);

            items.Add(new CheckoutDto
            {
                ProductId = new ProductId(productId),
                SellerId = new UserId(sellerId),
                Cost = Money.Create(currency, price)!,
                Description = lineItem.Description,
                Name = lineItem.Description,
                Quantity = quantity == 0 ? 1 : quantity,
                ImageUrl = lineItem.Price?.Product?.Images?.FirstOrDefault() ?? string.Empty
            });
        }

        return items;
    }

    private static bool TryGetUserId(
        Dictionary<string, string> metadata,
        out UserId userId)
    {
        if (metadata.TryGetValue("UserId", out var userIdValue) &&
            Guid.TryParse(userIdValue, out var parsed) || metadata.TryGetValue("AccountId", out var customerIdValue) &&
            Guid.TryParse(customerIdValue, out parsed))
        {
            userId = new UserId(parsed);
            return true;
        }

        userId = new UserId(Guid.Empty);
        return false;
    }

    private static bool TryGetTransactionId(
        Dictionary<string, string> metadata,
        out TransactionId transactionId)
    {
        if (metadata.TryGetValue("TransactionId", out var transactionIdValue) &&
            Guid.TryParse(transactionIdValue, out var transactionGuid))
        {
            transactionId = new TransactionId(transactionGuid);
            return true;
        }

        transactionId = new TransactionId(Guid.Empty);
        return false;
    }
}
