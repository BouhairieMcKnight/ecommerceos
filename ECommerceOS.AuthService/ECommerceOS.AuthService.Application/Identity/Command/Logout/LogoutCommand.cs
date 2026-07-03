namespace ECommerceOS.AuthService.Application.Identity.Command.Logout;

public record LogoutCommand(string? RefreshToken) : ICommand;
