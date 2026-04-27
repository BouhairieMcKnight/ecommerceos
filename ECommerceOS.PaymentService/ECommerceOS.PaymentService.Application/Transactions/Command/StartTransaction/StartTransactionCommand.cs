using ECommerceOS.Shared.DTOs;

namespace ECommerceOS.PaymentService.Application.Transactions.Command.StartTransaction;

public record StartTransactionCommand(
    Guid IdempotentCommandId,
    UserId? UserId,
    IEnumerable<CheckoutDto> CheckoutDtos) 
    : ICommand<string>, IIdempotentCommand;