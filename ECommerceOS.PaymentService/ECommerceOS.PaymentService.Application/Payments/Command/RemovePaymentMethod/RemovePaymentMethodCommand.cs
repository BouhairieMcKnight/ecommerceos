namespace ECommerceOS.PaymentService.Application.Payments.Command.RemovePaymentMethod;

public record RemovePaymentMethodCommand(UserId? UserId, PaymentId? PaymentId) : ICommand;
