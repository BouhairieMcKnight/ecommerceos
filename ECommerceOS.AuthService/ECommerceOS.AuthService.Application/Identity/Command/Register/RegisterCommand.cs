namespace ECommerceOS.AuthService.Application.Identity.Command.Register;

public record RegisterCommand(
    string? Email,
    string? Username,
    string? Password,
    bool External,
    Role Role) : ICommand;
