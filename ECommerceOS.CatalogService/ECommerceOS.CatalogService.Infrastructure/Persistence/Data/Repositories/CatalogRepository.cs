namespace ECommerceOS.CatalogService.Infrastructure.Persistence.Data.Repositories;

public class CatalogRepository(CatalogDbContext dbContext) : ICatalogRepository
{
    public async Task<Result<Product>> GetByIdAsync(ProductId id, CancellationToken ct = new CancellationToken())
    {
        var product = await dbContext.Set<Product>().FirstOrDefaultAsync(p => p.Id == id, ct);

        return product is null ? Result<Product>.Failure(ProductErrors.NotFound) : Result<Product>.Success(product);
    }

    public async Task AddAsync(Product entity, CancellationToken ct = new CancellationToken())
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteInTransactionAsync(async (token) =>
        {
            await dbContext.Set<Product>().AddAsync(entity, token); 
            await dbContext.SaveChangesAsync(token);
        }, async (token) =>
        {
            return await dbContext.Set<Product>().AnyAsync(t => t.Id == entity.Id, token);
        }, ct);
    }

    public async Task UpdateAsync(Product entity, CancellationToken ct = new CancellationToken())
    {
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Product entity, CancellationToken ct = new CancellationToken())
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteInTransactionAsync(async (token) =>
        {
            dbContext.Set<Product>().Remove(entity); 
            await dbContext.SaveChangesAsync(token);
        }, async (token) =>
        {
            return await dbContext.Set<Product>().AnyAsync(t => t.Id == entity.Id, token);
        }, ct);
    }

    public async Task<bool> VerifyProductAsync(ProductId id, CancellationToken cancellationToken)
    {
        return await dbContext.Set<Product>().AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<bool> VerifySkuAsync(Sku sku, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<Product>().AnyAsync(p => p.Sku == sku, cancellationToken);
    }

    public async Task<bool> VerifySellerProductAsync(ProductId id, UserId userId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<Product>().AnyAsync(p => p.SellerId == userId && p.Id == id, cancellationToken);
    }

    public async Task<Result<IEnumerable<Product>>> GetProductsAsync(Sku sku, CancellationToken cancellationToken = default)
    {
        var products = await dbContext.Set<Product>()
            .Where(p => p.Sku == sku)
            .ToListAsync(cancellationToken);
        
        return Result<IEnumerable<Product>>.Success(products);
    }

    public async Task UpdateProductsAsync(IEnumerable<Product> products,
        CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Result<IQueryable<Product>>> GetProductsPaginatedAsync(
        Expression<Func<Product, object>> columnSelector,
        CategoryId? searchCategory = null,
        string? sortOrder = "",
        string? searchTerm = "",
        CancellationToken cancellationToken = default)
    {
        IQueryable<Product> productsQuery = dbContext.Set<Product>()
            .AsNoTracking();
        
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            productsQuery = productsQuery
                .Where(p =>
                    p.Name.Contains(searchTerm!) ||
                    p.Sku.Value.Contains(searchTerm)); 
        }

        if (searchCategory is not null)
        {
            productsQuery = productsQuery.Where(p =>
                p.ProductCategories.Any(pc => pc.CategoryId == searchCategory));
        }

        if (!string.IsNullOrEmpty(sortOrder) && sortOrder?.ToLower() == "desc")
        {
            productsQuery = productsQuery.OrderByDescending(columnSelector);
        }
        else
        {
            productsQuery = productsQuery.OrderBy(columnSelector);
        }

        return Task.FromResult(Result<IQueryable<Product>>.Success(productsQuery));
    }

    public async Task<Result> ReserveInventoryAsync(
        Dictionary<ProductId, int> requestedProducts, CancellationToken cancellationToken = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        var productIds = requestedProducts.Keys.ToList();
        
        var affected = await dbContext.Set<Product>()
            .Where(p => productIds.Contains(p.Id))
            .Where(p => p.Quantity >= requestedProducts[p.Id])
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(p => 
                    p.Quantity, p => p.Quantity - requestedProducts[p.Id]), cancellationToken: cancellationToken);
        
        if (affected != requestedProducts.Count)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(ProductErrors.NotFound);
        }
        
        await transaction.CommitAsync(cancellationToken);
        
        return Result.Success();
    }
}
