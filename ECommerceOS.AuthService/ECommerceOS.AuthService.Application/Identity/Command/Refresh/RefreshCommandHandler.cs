namespace ECommerceOS.AuthService.Application.Identity.Command.Refresh;

public class RefreshCommandHandler(
    IUserRepository userRepository,
    ITokenGenerator tokenGenerator)
    : ICommandHandler<RefreshCommand, RefreshCommandResponse>
{
    public async Task<Result<RefreshCommandResponse>> Handle(
        RefreshCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId!, cancellationToken);

        var result = await user
            .Bind(u => ValidateRefreshToken(u, request.RefreshToken))
            .Bind(u => u.RevokeRefreshToken(request.RefreshToken!))
            .Bind(tokenGenerator.GenerateTokens)
            .Bind(tokens => AttachRefreshToken(user.Value!, tokens))
            .TapAsync(async _ => await userRepository.UpdateAsync(user.Value!, cancellationToken));
        
        return result.Match(
            success => Result<RefreshCommandResponse>.Success(
                new RefreshCommandResponse(success.AccessToken, success.RefreshToken)),
            Result<RefreshCommandResponse>.Failure);
    }

    private static Result<User> ValidateRefreshToken(User user, string? refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Result<User>.Failure(IdentityErrors.NotValidCustomer);
        }

        var token = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);

        if (token is null || token.IsRevoked || token.IsExpired)
        {
            return Result<User>.Failure(IdentityErrors.NotValidCustomer);
        }

        return Result<User>.Success(user);
    }

    private static Result<(string AccessToken, string RefreshToken)> AttachRefreshToken(
        User user,
        (string AccessToken, string RefreshToken) tokens)
    {
        var createRefreshToken = user.CreateRefreshToken(tokens.RefreshToken);

        return createRefreshToken.IsSuccess
            ? Result<(string AccessToken, string RefreshToken)>.Success(tokens)
            : Result<(string AccessToken, string RefreshToken)>.Failure(createRefreshToken.Error!);
    }
}
