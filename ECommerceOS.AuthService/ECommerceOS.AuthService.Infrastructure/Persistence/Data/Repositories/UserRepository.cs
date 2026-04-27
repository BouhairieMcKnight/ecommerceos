namespace ECommerceOS.AuthService.Infrastructure.Persistence.Data.Repositories;

public class UserRepository(IdentityDbContext dbContext) : IUserRepository
{
    public async Task<Result<User>> GetByIdAsync(UserId id, CancellationToken ct = new CancellationToken())
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        
        return user is null ? Result<User>.Failure(IdentityErrors.CustomerNotFound) : Result<User>.Success(user);
    }

    public async Task AddAsync(User entity, CancellationToken ct = new CancellationToken())
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteInTransactionAsync(async (token) =>
        {
            await dbContext.Set<User>().AddAsync(entity, token); 
            await dbContext.SaveChangesAsync(token);
        }, async (token) =>
        {
            return await dbContext.Set<User>().AnyAsync(t => t.Id == entity.Id, token);
        }, ct);
    }

    public async Task UpdateAsync(User entity, CancellationToken ct = new CancellationToken())
    {
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(User entity, CancellationToken ct = default)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteInTransactionAsync(async (token) =>
        {
            dbContext.Set<User>().Remove(entity); 
            await dbContext.SaveChangesAsync(token);
        }, async (token) =>
        {
            return await dbContext.Set<User>().AnyAsync(t => t.Id == entity.Id, token);
        }, ct);
    }

    public async Task<Result<User>> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        
        return user is null ? Result<User>.Failure(IdentityErrors.CustomerNotFound) : Result<User>.Success(user);
    }

    public async Task<bool> IsValidUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users.AnyAsync(user => user.Id == userId, cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken ct = default)
    {
        return !await dbContext.Users.AnyAsync(user => user.Email == email, ct);
    }

    public async Task UpdateUserVerificationAsync(UserId userId)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        
    }

    public async Task<bool> IsValidRefreshTokenAsync(string refreshTokenId, CancellationToken ct = default)
    {
        return await dbContext.Users
            .SelectMany(user => user.RefreshTokens)
            .AnyAsync(token => token.Token == refreshTokenId, ct);
    }
}
