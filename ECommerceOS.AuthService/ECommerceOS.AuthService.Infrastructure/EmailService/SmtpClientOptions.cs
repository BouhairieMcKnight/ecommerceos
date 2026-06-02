namespace ECommerceOS.AuthService.Infrastructure.EmailService;

public record SmtpClientOptions
{
    public string EmailAddress { get; init; }
    public string Password { get; init; }
    public string Host { get; init; }
    public int Port { get; init; }
}