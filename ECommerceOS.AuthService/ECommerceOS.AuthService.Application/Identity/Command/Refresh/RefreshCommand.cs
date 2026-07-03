namespace ECommerceOS.AuthService.Application.Identity.Command.Refresh;

public record RefreshCommand(string? RefreshToken) : ICommand<RefreshCommandResponse>;

public record RefreshCommandResponse(string AccessToken, string RefreshToken);
