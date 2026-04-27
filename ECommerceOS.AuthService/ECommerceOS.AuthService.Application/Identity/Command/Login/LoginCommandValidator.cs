namespace ECommerceOS.AuthService.Application.Identity.Command.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator(IUserRepository userRepository)
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Email is required");

        RuleFor(command => command.Email)
            .MustAsync(async (email, ct) => !await userRepository.IsEmailUniqueAsync(email, ct))
            .WithMessage("Email does not exist");
        
        RuleFor(command => command.Password)
            .NotEmpty()
            .WithMessage("Password is required");
    }
}
