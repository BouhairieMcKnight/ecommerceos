namespace ECommerceOS.AuthService.Application.Common.Interfaces;

public interface IPasswordHasher
{
    Result<string> Hash(string password);

    Result Verify(string password, string hashedPassword);
}