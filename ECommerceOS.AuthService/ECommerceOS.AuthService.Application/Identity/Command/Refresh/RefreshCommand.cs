namespace ECommerceOS.AuthService.Application.Identity.Command.Refresh;

public record RefreshCommand(UserId? UserId, string? RefreshToken) : ICommand<RefreshCommandResponse>;

public record RefreshCommandResponse(string AccessToken, string RefreshToken);
