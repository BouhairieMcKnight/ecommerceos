namespace ECommerceOS.AuthService.Application.Identity.Query.User;

public class UserQueryValidator : AbstractValidator<UserQuery>
{
    public UserQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotNull()
            .WithMessage("User Id cannot be null");
    }
}