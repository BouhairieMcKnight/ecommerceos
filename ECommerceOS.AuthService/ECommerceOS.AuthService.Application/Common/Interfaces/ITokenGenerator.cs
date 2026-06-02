namespace ECommerceOS.AuthService.Application.Common.Interfaces;

public interface ITokenGenerator
{
    public Result<(string AccessToken, string RefreshToken)> GenerateTokens(User user);
}