using ECommerceOS.PaymentService.Presentation.Payment.Cancel;
using ECommerceOS.PaymentService.Presentation.Payment.Get;
using ECommerceOS.PaymentService.Presentation.Payment.InitializePayment;

namespace ECommerceOS.PaymentService.Presentation.Payment;

public static class PaymentExtensions
{
    private const string RoutePrefix = "/payment";

    public static RouteGroupBuilder MapPaymentsGroup(this WebApplication app)
    {
        var group = app.MapGroup(RoutePrefix);

        group.MapInitializePaymentEndpoint();
        group.MapGetByIdEndpoint();
        group.MapDeleteEndpoint();

        return group;
    }
}
