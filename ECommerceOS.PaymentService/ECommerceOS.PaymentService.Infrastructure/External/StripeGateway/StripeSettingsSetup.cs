namespace ECommerceOS.PaymentService.Infrastructure.External.StripeGateway;

public sealed record StripeSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
}

public sealed class StripeSettingsSetup(
    IConfiguration configuration) 
    : IConfigureOptions<StripeSettings>
{
    public void Configure(StripeSettings options)
    {
        configuration.GetSection("StripeWebhookOptions").Bind(options);
    }
}

public sealed class StripeClientOptionsSetup(IOptions<StripeSettings> stripeOptions)
    : IConfigureOptions<StripeClientOptions>
{
    public void Configure(StripeClientOptions options)
    {
        var stripeClientOptions = stripeOptions.Value;
        
        options.ApiKey = stripeClientOptions.ApiKey;
    }
}