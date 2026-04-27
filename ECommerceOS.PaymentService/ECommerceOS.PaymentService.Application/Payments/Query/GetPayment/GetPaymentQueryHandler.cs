namespace ECommerceOS.PaymentService.Application.Payments.Query.GetPayment;

public class GetPaymentQueryHandler(IPaymentRepository paymentRepository)
    : IQueryHandler<GetPaymentQuery, GetPaymentQueryResponse>
{
    public async Task<Result<GetPaymentQueryResponse>> Handle(
        GetPaymentQuery request,
        CancellationToken cancellationToken)
    {
        var paymentResult = await paymentRepository.GetByIdAsync(request.PaymentId!, cancellationToken);
        return paymentResult.Bind(payment => Result<GetPaymentQueryResponse>.Success(
            new GetPaymentQueryResponse(
                payment.Id,
                payment.UserId,
                payment.PaymentMetadata.Id,
                payment.PaymentMetadata.PaymentMethod,
                payment.PaymentStatus.ToString())));
    }
}
