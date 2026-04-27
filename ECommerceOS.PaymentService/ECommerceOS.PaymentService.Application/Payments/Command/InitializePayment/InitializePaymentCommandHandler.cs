namespace ECommerceOS.PaymentService.Application.Payments.Command.InitializePayment;

public class InitializePaymentCommandHandler(
    IPaymentService paymentService) 
    : ICommandHandler<InitializePaymentCommand, string>
{
    public async Task<Result<string>> Handle(
        InitializePaymentCommand request,
        CancellationToken cancellationToken)
    {
        var result = await paymentService
            .RegisterPaymentMethodAsync(request.UserId!, request.PaymentMethod, cancellationToken);
        
        return result;
    }
}