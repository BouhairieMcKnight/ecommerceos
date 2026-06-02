namespace ECommerceOS.AuthService.Infrastructure.Authentication;

public class TokenGenerator(IOptions<JwtOptions> options) : ITokenGenerator
{
    public Result<(string AccessToken, string RefreshToken)> GenerateTokens(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var issuedAt = DateTimeOffset.UtcNow;
        var expiresAt = issuedAt.AddMinutes(options.Value.ExpiresInMinutes);

        List<Claim> claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim("verified", user.IsEmailVerified.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        var descriptor = new SecurityTokenDescriptor()
        {
            Subject = new(claims),
            IssuedAt = issuedAt.UtcDateTime,
            Expires = expiresAt.UtcDateTime,
            Issuer = options.Value.Issuer,
            Audience = options.Value.Audience,
            SigningCredentials = new(options.Value.Key, 
                SecurityAlgorithms.HmacSha256Signature)
        };
        
        var securityToken = tokenHandler.CreateJwtSecurityToken(descriptor);
        var jwtToken = tokenHandler.WriteToken(securityToken);

        if (string.IsNullOrEmpty(jwtToken))
        {
            return Result<(string AccessToken, string RefreshToken)>.Failure(IdentityErrors.NotValidCustomer);
        }

        var refreshToken = GenerateRefreshToken();

        return Result<(string AccessToken, string RefreshToken)>.Success((jwtToken, refreshToken));
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(randomBytes);
    }
}
