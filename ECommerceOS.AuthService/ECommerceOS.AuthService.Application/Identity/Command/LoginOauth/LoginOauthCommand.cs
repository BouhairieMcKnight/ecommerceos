namespace ECommerceOS.AuthService.Application.Identity.Command.LoginOauth;

public record LoginOauthCommand(string? Email, string? Username) : ICommand<LoginOauthCommandResponse>;

public record LoginOauthCommandResponse(string AccessToken, string RefreshToken)
{
    public static LoginOauthCommandResponse Create(string accessToken, string refreshToken)
    {
        return new(accessToken, refreshToken);
    }
};
