namespace ECommerceOS.AuthService.Application.Identity.Command.Delete;

public class DeleteCommandHandler(
    IUserRepository userRepository)
    : ICommandHandler<DeleteCommand>
{
    public async Task<Result> Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        var result = await userRepository.GetByIdAsync(request.UserId!, cancellationToken)
            .TapAsync(u => userRepository.DeleteAsync(u, cancellationToken));

        return result.Match(
            success => Result.Success(),
            Result.Failure);
    }
}
