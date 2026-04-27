namespace ECommerceOS.AuthService.Application.Identity.Command.LoginOauth;

public class LoginOauthCommandHandler(
    IUserRepository userRepository,
    ITokenGenerator authService)
    : ICommandHandler<LoginOauthCommand, LoginOauthCommandResponse>
{
    public async Task<Result<LoginOauthCommandResponse>> Handle(
        LoginOauthCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByEmailAsync(request.Email!, cancellationToken);
        
        var result = await user.Bind(authService.GenerateTokens)
            .Bind(tokens => AttachRefreshToken(user.Value!, tokens))
            .TapAsync(async _ => await userRepository.UpdateAsync(user.Value!, cancellationToken));

        return result.Match(
            success => Result<LoginOauthCommandResponse>
                .Success(LoginOauthCommandResponse.Create(success.AccessToken, success.RefreshToken)),
            Result<LoginOauthCommandResponse>.Failure);
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
