namespace ECommerceOS.AuthService.Application.Identity.Command.LoginOauth;

public class LoginOauthCommandValidator : AbstractValidator<LoginOauthCommand>
{
    public  LoginOauthCommandValidator()
    {
        RuleFor(c => c.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Email is required");
        
        RuleFor(c => c.Username)
            .NotEmpty()
            .WithMessage("Username is required");
    }
}
