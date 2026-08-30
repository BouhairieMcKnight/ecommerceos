namespace ECommerceOS.AuthService.Application.Identity.RefreshTokens;

public sealed class RefreshTokenSessionService : IRefreshTokenSessionService
{
    public Result<(string AccessToken, string RefreshToken)> StartSession(
        User user,
        (string AccessToken, string RefreshToken) tokens)
    {
        var result = user.CreateRefreshToken(tokens.RefreshToken);

        return result.IsSuccess
            ? Result<(string AccessToken, string RefreshToken)>.Success(tokens)
            : Result<(string AccessToken, string RefreshToken)>.Failure(result.Error!);
    }

    public Result<(string AccessToken, string RefreshToken)> RotateSession(
        User user,
        string currentRefreshToken,
        (string AccessToken, string RefreshToken) tokens)
    {
        var result = user.RotateRefreshToken(currentRefreshToken, tokens.RefreshToken);

        return result.IsSuccess
            ? Result<(string AccessToken, string RefreshToken)>.Success(tokens)
            : Result<(string AccessToken, string RefreshToken)>.Failure(result.Error!);
    }

    public Result EndSession(User user, string refreshToken)
    {
        return user.DeleteRefreshTokenFamily(refreshToken);
    }
}
