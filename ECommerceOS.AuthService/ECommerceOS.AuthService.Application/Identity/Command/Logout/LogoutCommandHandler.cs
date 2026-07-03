namespace ECommerceOS.AuthService.Application.Identity.Command.Logout;

public class LogoutCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenSessionService refreshTokenSessionService)
    : ICommandHandler<LogoutCommand>
{
    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return Result.Failure(IdentityErrors.NotValidCustomer);
        }

        var userResult = await userRepository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (!userResult.IsSuccess)
        {
            return Result.Failure(userResult.Error!);
        }

        var user = userResult.Value!;
        var result = refreshTokenSessionService.EndSession(user, request.RefreshToken);

        await userRepository.UpdateAsync(user, cancellationToken);

        return result.IsSuccess ? Result.Success() : Result.Failure(result.Error!);
    }
}
