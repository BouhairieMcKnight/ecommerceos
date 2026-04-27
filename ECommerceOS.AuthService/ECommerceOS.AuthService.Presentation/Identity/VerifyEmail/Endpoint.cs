using ECommerceOS.AuthService.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ZiggyCreatures.Caching.Fusion;

namespace ECommerceOS.AuthService.Api.Identity.VerifyEmail;

public static class Endpoint
{
    private const string Route = "/verify";

    public static RouteHandlerBuilder MapVerificationEndpoint(this RouteGroupBuilder builder)
    {
        return builder.MapPost(Route, HandleAsync);
    }

    private static async Task<IResult> HandleAsync(
        [FromQuery(Name = "url")] string verificationCodeAddress,
        [FromBody] string inputCode,
        [FromServices] IEmailSender emailSender,
        [FromServices] IFusionCache cache,
        [FromServices] IEncryptionService encryptionService,
        [FromServices] IUserRepository userRepository,
        [FromServices] ILogger logger,
        CancellationToken cancellationToken)
    {
        var encryptedKey = Uri.UnescapeDataString(verificationCodeAddress);
        
        var decryptedKey = encryptionService.Decrypt(encryptedKey);
        
        var parts = decryptedKey.Split(' ');
        
        if (parts.Length != 2 ||
            !Guid.TryParse(parts[0], out var userGuid))
        {
            logger.LogError("Could not parse verification code address");
            return Results.BadRequest();
        }
        
        var code = await cache.GetOrDefaultAsync(decryptedKey, string.Empty, token: cancellationToken);

        if (string.IsNullOrEmpty(code) || code != inputCode)
        {
            logger.LogError("Verification code not valid");
            return Results.BadRequest();
        }

        var result = await  userRepository.GetByIdAsync(new UserId(userGuid), cancellationToken)
            .Bind(u => u.VerifyEmail())
            .TapAsync(async u => await userRepository.UpdateAsync(u, cancellationToken));
        

        return result.IsSuccess ? Results.Ok() : result.ToProblemDetails();
    }
}