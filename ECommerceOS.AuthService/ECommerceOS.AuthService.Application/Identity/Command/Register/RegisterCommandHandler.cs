namespace ECommerceOS.AuthService.Application.Identity.Command.Register;

public class RegisterCommandHandler(IUserRepository userRepository,
    IPasswordHasher passwordHasher)
    : ICommandHandler<RegisterCommand>
{
    public async Task<Result> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {

        var result = await CreatePasswordHash(request.Password)
            .Bind(p => User.Create(userName: request.Username!, role: request.Role, email: request.Email!,
                password: p, isEmailVerified: request.External))
            .TapAsync(async u => await userRepository.AddAsync(u, cancellationToken));
            
        return result.Match(
                success => Result.Success(),
                Result.Failure);
    }

    private Result<string> CreatePasswordHash(string? password)
    {
        return string.IsNullOrEmpty(password) ? Result<string>.Success(string.Empty) : passwordHasher.Hash(password);
    }
}
