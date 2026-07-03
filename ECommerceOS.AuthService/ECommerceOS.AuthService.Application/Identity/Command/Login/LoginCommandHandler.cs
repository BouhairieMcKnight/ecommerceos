namespace ECommerceOS.AuthService.Application.Identity.Command.Login;

public class LoginCommandHandler(
    IPasswordHasher passwordHasher,
    ITokenGenerator authService,
    IUserRepository userRepository,
    IRefreshTokenSessionService refreshTokenSessionService)
    : ICommandHandler<LoginCommand, LoginCommandResponse>
{
    public async Task<Result<LoginCommandResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByEmailAsync(request.Email, cancellationToken)
            .Bind(u => VerifyPassword(request.Password, u));

        var result = await user.Bind(authService.GenerateTokens)
            .Bind(tokens => refreshTokenSessionService.StartSession(user.Value!, tokens))
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

}
