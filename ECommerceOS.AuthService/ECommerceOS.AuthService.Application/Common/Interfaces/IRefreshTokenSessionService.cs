namespace ECommerceOS.AuthService.Application.Common.Interfaces;

public interface IRefreshTokenSessionService
{
    Result<(string AccessToken, string RefreshToken)> StartSession(
        User user,
        (string AccessToken, string RefreshToken) tokens);

    Result<(string AccessToken, string RefreshToken)> RotateSession(
        User user,
        string currentRefreshToken,
        (string AccessToken, string RefreshToken) tokens);

    Result EndSession(User user, string refreshToken);
}
