using ECommerceOS.Shared.ValueObjects;

namespace ECommerceOS.AuthService.Application.Identity.Command.Logout;

public class LogoutCommandHandler(
    IUserRepository userRepository) 
    : ICommandHandler<LogoutCommand>
{
    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var result = await userRepository.GetByIdAsync(request.UserId!, cancellationToken)
            .Bind(u => u.ClearRefreshTokens())
            .TapAsync(async u => await userRepository.UpdateAsync(u, cancellationToken));

        return result.Match(
            success => Result.Success(),
            Result.Failure);
    }
}