namespace ECommerceOS.AuthService.Infrastructure.Persistence.Data.Repositories;

public class UnitOfWork(
    IdentityDbContext dbContext,
    IUserRepository userRepository) : IUnitOfWork
{
    public void Dispose()
    {
        dbContext.Dispose();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        return await dbContext.SaveChangesAsync(cancellationToken);
    }
}