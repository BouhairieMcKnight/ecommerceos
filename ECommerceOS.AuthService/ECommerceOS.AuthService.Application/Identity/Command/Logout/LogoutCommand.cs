namespace ECommerceOS.AuthService.Application.Identity.Command.Logout;

public record LogoutCommand(UserId? UserId) : ICommand;