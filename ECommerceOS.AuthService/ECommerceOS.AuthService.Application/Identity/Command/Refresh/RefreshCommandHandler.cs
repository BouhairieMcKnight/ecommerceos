namespace ECommerceOS.AuthService.Application.Identity.Command.Refresh;

public class RefreshCommandHandler(
    IUserRepository userRepository,
    ITokenGenerator tokenGenerator,
    IRefreshTokenSessionService refreshTokenSessionService)
    : ICommandHandler<RefreshCommand, RefreshCommandResponse>
{
    public async Task<Result<RefreshCommandResponse>> Handle(
        RefreshCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return Result<RefreshCommandResponse>.Failure(IdentityErrors.NotValidCustomer);
        }

        var userResult = await userRepository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (!userResult.IsSuccess)
        {
            return Result<RefreshCommandResponse>.Failure(userResult.Error!);
        }

        var user = userResult.Value!;
        var tokenResult = tokenGenerator.GenerateTokens(user);

        if (!tokenResult.IsSuccess)
        {
            return Result<RefreshCommandResponse>.Failure(tokenResult.Error!);
        }

        var rotationResult = refreshTokenSessionService.RotateSession(
            user,
            request.RefreshToken,
            tokenResult.Value!);

        await userRepository.UpdateAsync(user, cancellationToken);

        return rotationResult.Match(
            success => Result<RefreshCommandResponse>.Success(
                new RefreshCommandResponse(success.AccessToken, success.RefreshToken)),
            Result<RefreshCommandResponse>.Failure);
    }
}
