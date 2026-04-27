using ECommerceOS.AuthService.Domain.Identity;
using ECommerceOS.Shared.ValueObjects;

namespace ECommerceOS.AuthService.Application.Common.Interfaces;

public interface IUserRepository : IRepository<User, UserId>
{
    Task<Result<User>> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> IsValidUserIdAsync(UserId user, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(string email, CancellationToken ct = default);
    Task<bool> IsValidRefreshTokenAsync(string refreshTokenId, CancellationToken ct = default);
}