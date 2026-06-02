namespace ECommerceOS.AuthService.Infrastructure.Saga;

public record VerifyUser
{
    public required UserId UserId { get; init; }
    public required string EmailAddress { get; init; }
    public required string Name { get; init; }
}

public record WelcomeUser
{
    public required UserId UserId { get; init; }
    public required string EmailAddress { get; init; }
    public required string Name { get; init; }
}
