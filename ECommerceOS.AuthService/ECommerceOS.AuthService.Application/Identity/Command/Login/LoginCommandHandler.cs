namespace ECommerceOS.AuthService.Application.Identity.Command.Login;

public class LoginCommandHandler(
    IPasswordHasher passwordHasher,
    ITokenGenerator authService,
    IUserRepository userRepository)
    : ICommandHandler<LoginCommand, LoginCommandResponse>
{
    public async Task<Result<LoginCommandResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByEmailAsync(request.Email, cancellationToken)
            .Bind(u => VerifyPassword(request.Password, u));

        var result = await user.Bind(authService.GenerateTokens)
            .Bind(tokens => AttachRefreshToken(user.Value!, tokens))
            .TapAsync(async _ => await userRepository.UpdateAsync(user.Value!, cancellationToken));

        return result.Match(
            success => Result<LoginCommandResponse>
                .Success(LoginCommandResponse.Create(success.AccessToken, success.RefreshToken)),
            Result<LoginCommandResponse>.Failure);
    }

    private Result<User> VerifyPassword(string password, User user)
    {
        if (string.IsNullOrWhiteSpace(user.Password))
        {
            return Result<User>.Failure(IdentityErrors.NotValidCustomer);
        }

        var result = passwordHasher.Verify(password, user.Password!);
        
        return result.IsSuccess ? Result<User>.Success(user) : Result<User>.Failure(result.Error!);
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
