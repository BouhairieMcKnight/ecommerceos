namespace ECommerceOS.PaymentService.Application.Payments.Query.GetPaginatedPayments;

public class GetPaginatedPaymentsQueryHandler(IPaymentRepository paymentRepository)
    : IQueryHandler<GetPaginatedPaymentsQuery, PaginatedList<GetPaginatedPaymentsQueryResponse>>
{
    public async Task<Result<PaginatedList<GetPaginatedPaymentsQueryResponse>>> Handle(
        GetPaginatedPaymentsQuery request,
        CancellationToken cancellationToken)
    {
        var query = paymentRepository.QueryByUser(request.CustomerId!)
            .Select(p => new GetPaginatedPaymentsQueryResponse(
                p.Id,
                p.PaymentMetadata.PaymentMethod,
                p.PaymentStatus.ToString(),
                p.PaymentMetadata.Id));

        var mapped = await PaginatedList<GetPaginatedPaymentsQueryResponse>.CreateAsync(
            query,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
        
        return Result<PaginatedList<GetPaginatedPaymentsQueryResponse>>.Success(mapped);
    }
}
