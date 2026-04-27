using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace ECommerceOS.AuthService.Infrastructure.Authentication;

public class GoogleClaimTransformation(IUserRepository userRepository) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = principal.Identities
            .FirstOrDefault(x => x.AuthenticationType == Schemes.GoogleOidcScheme);

        if (identity is null)
        {
            return principal;
        }
        
        var email = identity.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
        {
            return principal;
        }

        var isUniqueEmail = await userRepository.IsEmailUniqueAsync(email);

        if (!isUniqueEmail)
        {
            return principal;
        }
        
        if (!identity.HasClaim(claim => claim is { Type: "new_user", Value: "y" }))
        {
            identity.AddClaim(new Claim("new_user", "y"));
        }

        return principal;
    }
}
