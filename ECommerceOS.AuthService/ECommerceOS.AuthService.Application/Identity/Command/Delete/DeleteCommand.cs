namespace ECommerceOS.AuthService.Application.Identity.Command.Delete;

public record DeleteCommand(UserId? UserId) : ICommand
{
    public DateTimeOffset CreateDate { get; init; } = DateTimeOffset.Now;
}
