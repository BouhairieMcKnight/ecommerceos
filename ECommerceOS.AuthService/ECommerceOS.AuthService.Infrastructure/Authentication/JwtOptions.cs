using System.Text;

namespace ECommerceOS.AuthService.Infrastructure.Authentication;

public class JwtOptions
{
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string Secret { get; init; } = string.Empty;
    public int ExpiresInMinutes { get; init; } = 10;
    public SymmetricSecurityKey Key => new(Encoding.UTF8.GetBytes(Secret));
}

public sealed record JwtOptionsSetup : IConfigureOptions<JwtOptions>
{
    private const string SectionName = nameof(JwtOptions);
    private readonly IConfiguration _configuration;

    public JwtOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(JwtOptions options)
    {
        _configuration.GetSection(SectionName).Bind(options);
    }
}

public sealed class JwtBearerOptionsSetup(IOptions<JwtOptions> jwtOptions)
    : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (!string.IsNullOrWhiteSpace(name) &&
            !string.Equals(name, Options.DefaultName, StringComparison.Ordinal) &&
            !string.Equals(name, Schemes.DefaultScheme, StringComparison.Ordinal))
        {
            return;
        }

        options.TokenValidationParameters = new()
        {
            ValidIssuer = _jwtOptions.Issuer,
            ValidAudience = _jwtOptions.Audience,
            IssuerSigningKey = _jwtOptions.Key,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }

    public void Configure(JwtBearerOptions options)
    {
        Configure(Options.DefaultName, options);
    }
}
