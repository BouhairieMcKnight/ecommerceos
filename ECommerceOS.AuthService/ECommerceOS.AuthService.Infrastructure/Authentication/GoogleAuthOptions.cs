using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;

namespace ECommerceOS.AuthService.Infrastructure.Authentication;

public sealed class GoogleAuthOptions
{
    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
    public string[] Scopes { get; init; } = ["openid", "profile", "email"];
    public string AccessType { get; init; } = "offline";
    public bool SaveTokens { get; init; } = true;
}

public sealed class GoogleAuthOptionsSetup(IConfiguration configuration)
    : IConfigureOptions<GoogleAuthOptions>
{
    private const string SectionName = "Authentication:Google";
    
    public void Configure(GoogleAuthOptions options)
    {
        configuration.GetSection(SectionName).Bind(options);
    }
}

public sealed class GoogleOptionsSetup(
    IOptions<GoogleAuthOptions> googleAuthOptions)
    : IConfigureNamedOptions<GoogleOptions>
{
    private const string Callback = "/auth/oauth/google-response";
    private readonly GoogleAuthOptions _googleAuthOptions = googleAuthOptions.Value;
    
    public void Configure(string? name, GoogleOptions options)
    {
        if (!string.IsNullOrWhiteSpace(name) &&
            !string.Equals(name, Options.DefaultName, StringComparison.Ordinal) &&
            !string.Equals(name, Schemes.GoogleOidcScheme, StringComparison.Ordinal))
        {
            return;
        }

        options.Scope.Clear();
        foreach (var scope in _googleAuthOptions.Scopes.Where(scope => !string.IsNullOrWhiteSpace(scope)))
        {
            options.Scope.Add(scope);
        }
        
        options.ClientId = _googleAuthOptions.ClientId;
        options.ClientSecret = _googleAuthOptions.ClientSecret;
        options.AccessType = _googleAuthOptions.AccessType;
        options.SaveTokens = _googleAuthOptions.SaveTokens;
        options.SignInScheme = Schemes.ExternalCookieScheme;
        options.CallbackPath = Callback;
        options.Events.OnRemoteFailure = context =>
        {
            context.Response.Redirect("/auth/oauth/login-google");
            context.HandleResponse();
            return Task.CompletedTask;
        };
    }

    public void Configure(GoogleOptions options)
    {
        Configure(Options.DefaultName, options);
    }
}
