namespace ECommerceOS.PaymentService.Application.Payments.Command.InitializePayment;

public record InitializePaymentCommand(
    Guid IdempotentCommandId,
    UserId? UserId,
    string PaymentMethod)
    : ICommand<string>, IIdempotentCommand;