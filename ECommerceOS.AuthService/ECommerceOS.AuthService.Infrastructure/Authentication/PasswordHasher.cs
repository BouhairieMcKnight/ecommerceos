using Error = ECommerceOS.Shared.Result.Error;

namespace ECommerceOS.AuthService.Infrastructure.Authentication;

public sealed class PasswordHasher : IPasswordHasher
{
    private const int HashLength = 32;
    private const int Iterations = 150_000;
    private const int SaltLength = 16;
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512;
    
    public Result<string> Hash(string? password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltLength);
        var has = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithm,
            HashLength);

        var hasBytes = new byte[SaltLength + HashLength];
        Buffer.BlockCopy(salt, 0, hasBytes, 0, SaltLength);
        Buffer.BlockCopy(has, 0, hasBytes, SaltLength, HashLength);

        return Result<string>.Success(Convert.ToBase64String(hasBytes));
    }

    public Result Verify(string password, string hashedPassword)
    {
        var hashedBytes = Convert.FromBase64String(hashedPassword);
        
        var salt = new byte[SaltLength];
        
        var storedHash = new byte[HashLength];
        Buffer.BlockCopy(hashedBytes, 0, storedHash, 0, HashLength);

        var computedHash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithm,
            HashLength);

        var result = CryptographicOperations.FixedTimeEquals(storedHash, computedHash);

        return result ? Result.Success()
            : Result.Failure(Error.Validation("Password Invalidation", "Password has invalid hash"));
    }
}