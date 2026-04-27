namespace ECommerceOS.AuthService.Application.Identity.Command.Login;

public record LoginCommand : ICommand<LoginCommandResponse>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTimeOffset CreateDate { get; private set; } = DateTimeOffset.Now;
}

public record LoginCommandResponse(string AccessToken, string RefreshToken)
{
    public static LoginCommandResponse Create(string accessToken, string refreshToken)
    {
        return new(AccessToken: accessToken, RefreshToken: refreshToken);
    }
}
