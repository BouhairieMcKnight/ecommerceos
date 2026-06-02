using ECommerceOS.PaymentService.Presentation.Transaction.Cancel;
using ECommerceOS.PaymentService.Presentation.Transaction.GetById;
using ECommerceOS.PaymentService.Presentation.Transaction.GetStatus;
using ECommerceOS.PaymentService.Presentation.Transaction.InitializeTransaction;

namespace ECommerceOS.PaymentService.Presentation.Transaction;

public static class TransactionExtensions
{
    private const string RoutePrefix = "/transaction";

    public static RouteGroupBuilder MapTransactionsGroup(this WebApplication app)
    {
        var group = app.MapGroup(RoutePrefix);

        group.MapCreateSessionEndpoint();
        group.MapGetByIdEndpoint();
        group.MapCancelEndpoint();
        group.MapGetStatusEndpoint();
        
        
        return group;
    }
}