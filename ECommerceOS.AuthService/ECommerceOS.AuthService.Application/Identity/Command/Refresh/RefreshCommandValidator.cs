namespace ECommerceOS.AuthService.Application.Identity.Command.Refresh;

public class RefreshCommandValidator : AbstractValidator<RefreshCommand>
{
    public RefreshCommandValidator(IUserRepository userRepository)
    {
        RuleFor(command => command.UserId)
            .NotNull()
            .MustAsync(async (id, ct) => id is not null && await userRepository.IsValidUserIdAsync(id, ct))
            .WithMessage("Invalid user id");

        RuleFor(command => command.RefreshToken)
            .NotEmpty()
            .MustAsync(async (token, ct) =>
                !string.IsNullOrWhiteSpace(token) && await userRepository.IsValidRefreshTokenAsync(token, ct))
            .WithMessage("Invalid refresh token");
    }
}
