namespace ECommerceOS.PaymentService.Application.Payments.Query.GetPaginatedPayments;

public record GetPaginatedPaymentsQuery(
    UserId? CustomerId,
    Money? Cost,
    int PageNumber,
    int PageSize) 
    : IQuery<PaginatedList<GetPaginatedPaymentsQueryResponse>>;
    
public record GetPaginatedPaymentsQueryResponse(
    PaymentId PaymentId,
    string PaymentMethod,
    string PaymentStatus,
    string PaymentNumber);
