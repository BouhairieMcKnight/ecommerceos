using ECommerceOS.PaymentService.Application.Common.Interfaces;
using ECommerceOS.Shared.DTOs;

namespace ECommerceOS.PaymentService.Infrastructure.External.StripeGateway;

public class StripeTransactionService(
    SessionService sessionService,
    RefundService refundService,
    PaymentIntentService paymentIntentService,
    PaymentDbContext dbContext) : ITransactionService
{
    private const string UiMode = "embedded";
    private const string Mode = "payment";
    
    private async Task<string> RefundTransactionAsync(
        TransactionId transactionId, Money amount, CancellationToken cancellationToken = default)
    {
        var paymentIntentId = await GetPaymentIntentIdAsync(transactionId, cancellationToken);
        return await CreateRefundAsync(paymentIntentId, amount, cancellationToken);
    }

    public async Task<string> RefundCheckoutAsync(
        TransactionId transactionId,
        CancellationToken cancellationToken = default)
    {
        var transaction = await dbContext.Set<Transaction>()
            .Include(t => t.TransactionItems)
            .FirstOrDefaultAsync(t => t.Id == transactionId, cancellationToken);

        if (transaction is null)
        {
            throw new InvalidOperationException($"Transaction {transactionId.Value} was not found for refund.");
        }

        if (!transaction.TransactionItems.Any())
        {
            throw new InvalidOperationException($"Transaction {transactionId.Value} has no line items to refund.");
        }

        var total = transaction.TransactionItems.Sum(item => item.Amount.Amount);
        var currency = transaction.TransactionItems.First().Amount.CurrencyValue.Code;
        var totalAmount = Money.Create(currency, total)
            ?? throw new InvalidOperationException($"Transaction {transactionId.Value} refund amount is invalid.");

        return await RefundTransactionAsync(transactionId, totalAmount, cancellationToken);
    }

    public async Task<Result<string>> CancelTransactionAsync(TransactionId transactionId, TransactionStatus status,
        CancellationToken cancellationToken = default)
    {
        _ = status;
        var paymentIntentId = await GetPaymentIntentIdAsync(transactionId, cancellationToken);
        var paymentIntent = await paymentIntentService.GetAsync(paymentIntentId, cancellationToken: cancellationToken);
        var paymentStatus = MapTransactionStatus(paymentIntent.Status);

        switch (paymentStatus)
        {
            case nameof(TransactionStatus.Pending) or nameof(TransactionStatus.Confirmed):
            {
                var canceledPayment = await paymentIntentService.CancelAsync(paymentIntentId, cancellationToken: cancellationToken);
                return Result<string>.Success(canceledPayment.Status);
            }
            case nameof(TransactionStatus.Completed):
            {
                var refundAmount = Money.Create(paymentIntent.Currency, (decimal)paymentIntent.AmountReceived);
                if (refundAmount is null)
                {
                    return Result<string>.Failure(TransactionErrors.NotValidOperation);
                }

                var refundStatus = await CreateRefundAsync(paymentIntentId, refundAmount, cancellationToken);
                return Result<string>.Success(refundStatus);
            }
            default:
                return Result<string>.Failure(TransactionErrors.NotValidOperation);
        }
    }
    
    public async Task<string> CreateCheckoutAsync(
        UserId userId, 
        IEnumerable<CheckoutDto> products,
        CancellationToken cancellationToken = default)
    {
        var transactionId = new TransactionId(Guid.NewGuid());
        var customerId =
            await dbContext.Set<StripeAccount>()
                .Where(a => a.UserId == userId)
                .Select(a => a.AccountId)
                .FirstOrDefaultAsync(cancellationToken);
        
        var priceOptions = products.Select(product => 
            (new SessionLineItemPriceDataOptions
        {
            UnitAmountDecimal = product.Cost.Amount,
            Currency = product.Cost.CurrencyValue.Code,
            ProductData = new SessionLineItemPriceDataProductDataOptions
            {
                Name = product.Name,
                Description = product.Description,
                Images = [product.ImageUrl],
                Metadata =
                {
                    ["ProductId"] = product.ProductId.Value.ToString("D"),
                    ["SellerId"] = product.SellerId.Value.ToString("D")
                }
            },
        }, product.Quantity));

        var idempotentKey = Guid.NewGuid().ToString("D");
        
        var sessionOptions = new Stripe.Checkout.SessionCreateOptions
        {
            Customer = customerId,
            LineItems = priceOptions.Select(price => new SessionLineItemOptions
            {
                PriceData = price.Item1,
                Quantity = price.Quantity,
            }).ToList(),
            
            Metadata =
            {
                ["AccountId"] = userId.Value.ToString("D"),
                ["IdempotencyKey"] = idempotentKey,
                ["TransactionId"] = transactionId.Value.ToString("D")
            },
            
            PaymentIntentData = new SessionPaymentIntentDataOptions
            {
                CaptureMethod = "manual",
                Metadata =
                {
                    ["TransactionId"] = transactionId.Value.ToString("D")
                }
            },
            
            ShippingAddressCollection = new SessionShippingAddressCollectionOptions
            {
                AllowedCountries = { "US", "CA", "GB" }
            },
            
            Mode = Mode,
            UiMode = UiMode
        };

        var requestOptions = new RequestOptions
        {
            IdempotencyKey = idempotentKey
        };
        
        var session = await sessionService.CreateAsync(sessionOptions, requestOptions, cancellationToken);

        return session.ClientSecret;
    }

    public async Task<string> GetTransactionStatusAsync(
        string sessionId,
        CancellationToken cancellationToken = default)
    {
        var session = await sessionService.GetAsync(sessionId, cancellationToken: cancellationToken);
        return session.Status ?? "unknown";
    }

    public async Task CapturePaymentAsync(
        TransactionId transactionId,
        long amount,

        Guid idempotencyKey,
        CancellationToken cancellationToken = default)
    {
        var paymentIntentId = await GetPaymentIntentIdAsync(transactionId, cancellationToken);

        var captureOptions = new PaymentIntentCaptureOptions
        {
            AmountToCapture = amount
        };

        var requestOptions = new RequestOptions
        {
            IdempotencyKey = idempotencyKey.ToString("D")
        };

        await paymentIntentService.CaptureAsync(paymentIntentId, captureOptions, requestOptions, cancellationToken);
    }

    private async Task<string> GetPaymentIntentIdAsync(
        TransactionId transactionId,
        CancellationToken cancellationToken)
    {
        var stripeTransaction = await dbContext.Set<StripeTransactionMetadata>()
            .Where(t => t.TransactionId == transactionId)
            .Select(t => t.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(stripeTransaction))
        {
            throw new InvalidOperationException($"No stripe transaction metadata found for transaction {transactionId.Value}.");
        }

        var session = await sessionService.GetAsync(stripeTransaction, cancellationToken: cancellationToken);
        var paymentIntentId = session.PaymentIntentId;
        if (string.IsNullOrWhiteSpace(paymentIntentId))
        {
            throw new InvalidOperationException($"No payment intent found for transaction {transactionId.Value}.");
        }

        return paymentIntentId;
    }

    private async Task<string> CreateRefundAsync(
        string paymentIntentId,
        Money amount,
        CancellationToken cancellationToken)
    {
        var options = new RefundCreateOptions
        {
            Amount = (long)amount.Amount,
            Currency = amount.CurrencyValue.Code,
            PaymentIntent = paymentIntentId,
        };

        var requestOptions = new RequestOptions
        {
            IdempotencyKey = Guid.NewGuid().ToString("D")
        };

        var refund = await refundService.CreateAsync(options, requestOptions, cancellationToken);
        return refund.Status;
    }

    private static string MapTransactionStatus(string? paymentIntentStatus)
    {
        return paymentIntentStatus switch
        {
            "requires_payment_method" => nameof(TransactionStatus.Pending),
            "requires_confirmation" => nameof(TransactionStatus.Pending),
            "requires_action" => nameof(TransactionStatus.Pending),
            "processing" => nameof(TransactionStatus.Confirmed),
            "requires_capture" => nameof(TransactionStatus.Pending),
            "canceled" => nameof(TransactionStatus.Cancelled),
            "succeeded" => nameof(TransactionStatus.Completed),
            _ => string.Empty
        };
    }
}
