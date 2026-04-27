using ECommerceOS.PaymentService.Application.Transactions.Command.StartTransaction;
using ECommerceOS.Shared.DTOs;

namespace ECommerceOS.PaymentService.Presentation.GrpcServices;

public class CheckoutService(ISender sender) : Checkout.CheckoutBase
{
    public override async Task<GetCheckoutResponse> GetCheckoutSession(
        GetCheckoutRequest request,
        ServerCallContext context)
    {
        var idempotencyId = new Guid(request.IdempotentId);
        var userId = new UserId(new Guid(request.UserId));
        IEnumerable<CheckoutDto> transactions = request.Items.Select(item =>
        {
            var transactionDto = new CheckoutDto
            {
                Description = item.Description,
                Name = item.Name,
                ProductId = new ProductId(Guid.Parse(item.ProductId)),
                Quantity = item.Quantity,
                Cost = Money.Create(item.Price.Currency, (decimal)item.Price.Amount)!,
                ImageUrl = item.ImageUrl,
                SellerId = new UserId(Guid.Parse(item.SellerId))
            };

            return transactionDto;
        }).ToList();

        var command = new StartTransactionCommand(idempotencyId, userId, transactions);
        var result = await sender.Send(command);
        
        var output = new GetCheckoutResponse();

        if (result.IsSuccess)
        {
            output.ClientSecret = result.Value;
        }
        
        return output;
    }
}