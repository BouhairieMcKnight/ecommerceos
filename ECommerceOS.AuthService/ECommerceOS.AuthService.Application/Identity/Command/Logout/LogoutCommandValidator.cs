namespace ECommerceOS.AuthService.Application.Identity.Command.Logout;

public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator(IUserRepository userRepository)
    {
        RuleFor(command => command.UserId)
            .NotNull()
            .MustAsync(async (id, ct) => id is not null && await userRepository.IsValidUserIdAsync(id, ct))
            .WithMessage("Invalid user id");
    }
}
