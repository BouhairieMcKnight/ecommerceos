namespace ECommerceOS.PaymentService.Application.Payments.Command.RemovePaymentMethod;

public class RemovePaymentMethodCommandHandler(
    IPaymentRepository paymentRepository,
    IPaymentService paymentService) 
    : ICommandHandler<RemovePaymentMethodCommand>
{
    public async Task<Result> Handle(RemovePaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var result = await paymentRepository.GetByIdAsync(request.PaymentId!, cancellationToken)
            .Bind(p => p.RemovePayment())
            .TapAsync(async p => await paymentService.DeletePaymentAsync(p.Id, cancellationToken))
            .TapAsync(async p => await paymentRepository.DeleteAsync(p, cancellationToken));
            
        return result.Match(
            success => Result.Success(),
            Result.Failure);
    }
}