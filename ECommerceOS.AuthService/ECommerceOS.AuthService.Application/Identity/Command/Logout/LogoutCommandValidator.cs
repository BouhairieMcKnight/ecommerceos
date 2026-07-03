namespace ECommerceOS.AuthService.Application.Identity.Command.Logout;

public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(command => command.RefreshToken)
            .NotEmpty()
            .WithMessage("Invalid refresh token");
    }
}
