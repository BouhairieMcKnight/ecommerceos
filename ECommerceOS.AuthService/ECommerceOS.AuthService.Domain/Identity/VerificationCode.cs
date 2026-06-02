namespace ECommerceOS.AuthService.Domain.Identity;

public record VerificationCode
{
    public Guid Value { get; init; }
    public required UserId UserId { get; init; }
}