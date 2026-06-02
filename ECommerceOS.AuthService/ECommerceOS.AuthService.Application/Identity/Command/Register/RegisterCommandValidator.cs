namespace ECommerceOS.AuthService.Application.Identity.Command.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    private const string PasswordPattern = @"^(?=.*\d)(?=.*[^\w\s]).{8,50}$";

    public RegisterCommandValidator(IUserRepository userRepository)
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress()
            .MustAsync(async (email, ct) => await userRepository.IsEmailUniqueAsync(email, ct))
            .WithMessage("Email already exists");

        RuleFor(command => command.Password)
            .NotEmpty()
            .Matches(PasswordPattern)
            .When(command => !command.External)
            .WithMessage("Password must have at least 8 characters and must not exceed 50 characters");
    }
}
