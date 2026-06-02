namespace ECommerceOS.AuthService.Application.Identity.Command.Delete;

public class DeleteCommandValidator : AbstractValidator<DeleteCommand>
{
    public DeleteCommandValidator(IUserRepository userRepository)
    {
        RuleFor(command => command.UserId)
            .NotNull()
            .MustAsync(async (id, ct) =>
                id is not null && await userRepository.IsValidUserIdAsync(id, ct))
            .WithMessage("Email address is not valid");
    }
}
