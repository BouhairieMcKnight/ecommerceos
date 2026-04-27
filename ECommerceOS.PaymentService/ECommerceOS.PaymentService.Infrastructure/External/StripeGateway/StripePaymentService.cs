using System.Globalization;
using ECommerceOS.PaymentService.Application.Common.Interfaces;
using ECommerceOS.PaymentService.Infrastructure.Persistence.Data.Models;
using ECommerceOS.Shared.Result;

namespace ECommerceOS.PaymentService.Infrastructure.External.StripeGateway;

public class StripePaymentService(
    SetupIntentService setupService,
    CustomerService customerService,
    PaymentDbContext dbContext,
    PaymentMethodService paymentMethodService) : IPaymentService
{
    public async Task<string> RegisterCustomerAsync(
        UserId userId, string email, string name, CancellationToken cancellationToken = default)
    {
        var customerOptions = new CustomerCreateOptions
        {
            Email = email,
            Name = name,
            Metadata = new Dictionary<string, string>
            {
                ["UserId"] = userId.Value.ToString("N")
            }
        };
        
        var customer = await customerService.CreateAsync(customerOptions, cancellationToken: cancellationToken);
        
        var account = StripeAccount.Create(userId, customer.Id);
        
        await dbContext.Set<StripeAccount>()
            .AddAsync(account, cancellationToken);
        
        await dbContext.SaveChangesAsync(cancellationToken);
        return customer.Id;
    }

    public async Task<Result> DeletePaymentAsync(PaymentId paymentId, CancellationToken cancellationToken = default)
    {
        var metadata = await dbContext.Set<PaymentMetadata>()
            .AsNoTracking()
            .Where(p => p.PaymentId == paymentId)
            .Select(p => p.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (metadata is null)
        {
            return Result.Failure(PaymentErrors.NotFoundByPaymentId(paymentId));
        }

        await paymentMethodService.DetachAsync(metadata, cancellationToken: cancellationToken);
        return Result.Success();
    }
    
    public async Task<List<string>> GetPaymentMethodsAsync(
        UserId userId, 
        string paymentMethod,
        CancellationToken cancellationToken = default)
    {
        var customerId =
            await dbContext.Set<StripeAccount>()
                .Where(a => a.UserId == userId)
                .Select(u => u.AccountId)
                .FirstOrDefaultAsync(cancellationToken);
        
        var customer = await customerService.GetAsync(customerId, cancellationToken: cancellationToken);
        
        var paymentMethodOptions = new PaymentMethodListOptions
        {
            Customer = customer.Id,
            Type = paymentMethod
        };
        
        var paymentMethods = await paymentMethodService
            .ListAsync(paymentMethodOptions, cancellationToken: cancellationToken);
        
        return paymentMethods.Select(p => p.Id).ToList();
    }

    public async Task<Result<string>> RegisterPaymentMethodAsync(
        UserId userId,
        string paymentMethodType,
        CancellationToken cancellationToken = default)
    {
        var customerId = await dbContext.Set<StripeAccount>()
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .Select(u => u.AccountId)
            .FirstOrDefaultAsync(cancellationToken);
        
        var options = new SetupIntentCreateOptions
        {
            PaymentMethodTypes = new List<string> { paymentMethodType },
            Customer = customerId,
            Usage = "off_session",
            Metadata = new Dictionary<string, string>
            {
                ["UserId"] = userId.Value.ToString("D")
            }
        };

        var requestOptions = new RequestOptions
        {
            IdempotencyKey = Guid.NewGuid().ToString()
        };

        var intent = await setupService.CreateAsync(options, requestOptions, cancellationToken);

        return Result<string>.Success(intent.ClientSecret ?? intent.Id);
    }
}
