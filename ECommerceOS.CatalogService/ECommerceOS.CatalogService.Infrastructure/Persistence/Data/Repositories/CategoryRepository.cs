using ECommerceOS.CatalogService.Application.Common.Models;
using Dapper;
using Microsoft.EntityFrameworkCore.Storage;

namespace ECommerceOS.CatalogService.Infrastructure.Persistence.Data.Repositories;

public class CategoryRepository(CatalogDbContext dbContext) : ICategoryRepository
{
    public async Task<Result<Category>> GetByIdAsync(CategoryId id, CancellationToken ct = new CancellationToken())
    {
        var category = await dbContext.Set<Category>().FirstOrDefaultAsync(c => c.Id == id, ct);
        
        return category is null ? Result<Category>.Failure(CategoryErrors.NotFound) : Result<Category>.Success(category);
    }

    public async Task AddAsync(Category entity, CancellationToken ct = new CancellationToken())
    {
        await ExecuteInTransactionAsync(async token =>
        {
            await dbContext.Set<Category>().AddAsync(entity, token);
            await dbContext.SaveChangesAsync(token);
            await RebuildCategoryClosuresAsync(token);
        }, token => dbContext.Set<Category>().AnyAsync(t => t.Id == entity.Id, token), ct);
    }

    public async Task UpdateAsync(Category entity, CancellationToken ct = new CancellationToken())
    {
        await ExecuteInTransactionAsync(async token =>
        {
            await dbContext.SaveChangesAsync(token);
            await RebuildCategoryClosuresAsync(token);
        }, _ => Task.FromResult(true), ct);
    }

    public async Task DeleteAsync(Category entity, CancellationToken ct = new CancellationToken())
    {
        await ExecuteInTransactionAsync(async token =>
        {
            dbContext.Set<Category>().Remove(entity);
            await dbContext.SaveChangesAsync(token);
            await RebuildCategoryClosuresAsync(token);
        }, async token => !await dbContext.Set<Category>().AnyAsync(t => t.Id == entity.Id, token), ct);
    }

    public async Task<Result<Category>> GetCategoryByNameAsync(string? categoryName, CancellationToken cancellationToken = default)
    {
        var category = await dbContext.Set<Category>()
            .FirstOrDefaultAsync(c => c.Title == categoryName, cancellationToken);

        return category is null
            ? Result<Category>.Failure(CategoryErrors.NotFound)
            : Result<Category>.Success(category);
    }

    public async Task<bool> VerifyCategoryNameAsync(string categoryName, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<Category>().AnyAsync(c => c.Title == categoryName, cancellationToken);
    }

    public async Task<bool> VerifyCategoryParentIdAsync(CategoryId parentId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<Category>().AnyAsync(t => t.Id == parentId, cancellationToken);
    }

    public async Task<bool> VerifyCategoryParentNameAsync(string parentName, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<Category>().AnyAsync(c => c.Title == parentName, cancellationToken);
    }

    public async Task<Result<(Category Category, CategoryId ParentId)>> GetCategoryTreeAsync(
        string categoryName,
        string parentCategoryName,
        CancellationToken cancellationToken = default)
    {
        await RebuildCategoryClosuresAsync(cancellationToken);

        var connection = dbContext.Database.GetDbConnection();
        var transaction = dbContext.Database.CurrentTransaction?.GetDbTransaction();

        var categories = (await connection.QueryAsync<CategoryLookupRow>(
            new CommandDefinition(
                """
                SELECT
                    id AS CategoryId,
                    title AS Title
                FROM categories
                WHERE title = @CategoryName OR title = @ParentCategoryName
                """,
                new { CategoryName = categoryName, ParentCategoryName = parentCategoryName },
                transaction: transaction,
                cancellationToken: cancellationToken))).AsList();
        var categoryLookup = categories.FirstOrDefault(c => c.Title == categoryName);
        var parentLookup = categories.FirstOrDefault(c => c.Title == parentCategoryName);

        if (categoryLookup == default || parentLookup == default)
        {
            return Result<(Category, CategoryId)>.Failure(CategoryErrors.NotFound);
        }

        var categoryId = new CategoryId(categoryLookup.CategoryId);
        var parentCategoryId = new CategoryId(parentLookup.CategoryId);

        var parentIsDescendant = categoryId == parentCategoryId ||
            await connection.ExecuteScalarAsync<bool>(new CommandDefinition(
                """
                SELECT EXISTS (
                    SELECT 1
                    FROM category_closures
                    WHERE parent_category_id = @CategoryId
                      AND child_category_id = @ParentCategoryId
                      AND depth > 0
                )
                """,
                new
                {
                    CategoryId = categoryId.Value,
                    ParentCategoryId = parentCategoryId.Value
                },
                transaction: transaction,
                cancellationToken: cancellationToken));

        if (parentIsDescendant)
        {
            return Result<(Category, CategoryId)>.Failure(CategoryErrors.NotValidCategory);
        }

        var category = await dbContext.Set<Category>()
            .FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken);

        if (category is null)
        {
            return Result<(Category, CategoryId)>.Failure(CategoryErrors.NotFound);
        }

        return Result<(Category, CategoryId)>.Success((category, parentCategoryId));
    }

    public async Task<Result<IReadOnlyCollection<CategoryHierarchyNode>>> GetCategoryHierarchyAsync(
        CancellationToken cancellationToken = default)
    {
        await RebuildCategoryClosuresAsync(cancellationToken);

        var connection = dbContext.Database.GetDbConnection();
        var categories = (await connection.QueryAsync<CategoryLookupRow>(
            new CommandDefinition(
                """
                SELECT
                    id AS CategoryId,
                    title AS Title
                FROM categories
                """,
                cancellationToken: cancellationToken))).AsList();

        if (categories.Count == 0)
        {
            return Result<IReadOnlyCollection<CategoryHierarchyNode>>.Success(Array.Empty<CategoryHierarchyNode>());
        }

        var directLinks = (await connection.QueryAsync<CategoryEdgeRow>(
            new CommandDefinition(
                """
                SELECT
                    parent_category_id AS ParentCategoryId,
                    child_category_id AS ChildCategoryId
                FROM category_closures
                WHERE depth = 1
                """,
                cancellationToken: cancellationToken))).AsList();

        var nodes = categories.ToDictionary(
            category => new CategoryId(category.CategoryId),
            category => new CategoryHierarchyBuilderNode(
                new CategoryId(category.CategoryId),
                category.Title));
        var hasParent = new HashSet<CategoryId>();

        foreach (var link in directLinks)
        {
            if (link.ParentCategoryId == link.ChildCategoryId)
            {
                continue;
            }

            var parentCategoryId = new CategoryId(link.ParentCategoryId);
            var childCategoryId = new CategoryId(link.ChildCategoryId);

            if (!nodes.TryGetValue(parentCategoryId, out var parentNode) ||
                !nodes.TryGetValue(childCategoryId, out var childNode))
            {
                continue;
            }
            
            parentNode.Children.Add(childNode);
            hasParent.Add(childNode.CategoryId);
        }

        var roots = nodes.Values
            .Where(node => !hasParent.Contains(node.CategoryId))
            .OrderBy(node => node.Title, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (roots.Count == 0)
        {
            roots = nodes.Values
                .OrderBy(node => node.Title, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        var hierarchy = roots
            .Select(root => MapToHierarchyNode(root, new HashSet<CategoryId>()))
            .ToList()
            .AsReadOnly();

        return Result<IReadOnlyCollection<CategoryHierarchyNode>>.Success(hierarchy);
    }

    private async Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> operation,
        Func<CancellationToken, Task<bool>> verifySucceeded,
        CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteInTransactionAsync(operation, verifySucceeded, cancellationToken);
    }
    
    private async Task RebuildCategoryClosuresAsync(CancellationToken cancellationToken)
    {
        var connection = dbContext.Database.GetDbConnection();
        var transaction = dbContext.Database.CurrentTransaction?.GetDbTransaction();

        await connection.ExecuteAsync(new CommandDefinition(
            """
            DELETE FROM category_closures;

            INSERT INTO category_closures(parent_category_id, child_category_id, depth)
            WITH RECURSIVE category_tree AS (
                SELECT
                    c.id AS parent_category_id,
                    c.id AS child_category_id,
                    0 AS depth,
                    ARRAY[c.id] AS path
                FROM categories c

                UNION ALL

                SELECT
                    tree.parent_category_id,
                    child.id AS child_category_id,
                    tree.depth + 1 AS depth,
                    tree.path || child.id
                FROM category_tree tree
                JOIN categories child ON child.parent_category_id = tree.child_category_id
                WHERE NOT (child.id = ANY(tree.path))
            )
            SELECT
                parent_category_id,
                child_category_id,
                MIN(depth) AS depth
            FROM category_tree
            GROUP BY parent_category_id, child_category_id;
            """,
            transaction: transaction,
            cancellationToken: cancellationToken));
    }

    private static CategoryHierarchyNode MapToHierarchyNode(
        CategoryHierarchyBuilderNode source,
        HashSet<CategoryId> recursionPath)
    {
        if (!recursionPath.Add(source.CategoryId))
        {
            return new CategoryHierarchyNode(source.CategoryId, source.Title, Array.Empty<CategoryHierarchyNode>());
        }

        var children = source.Children
            .OrderBy(child => child.Title, StringComparer.OrdinalIgnoreCase)
            .Select(child => MapToHierarchyNode(child, recursionPath))
            .ToList()
            .AsReadOnly();

        recursionPath.Remove(source.CategoryId);
        return new CategoryHierarchyNode(source.CategoryId, source.Title, children);
    }

    private readonly record struct CategoryLookupRow(Guid CategoryId, string Title);
    private readonly record struct CategoryEdgeRow(Guid ParentCategoryId, Guid ChildCategoryId);

    private sealed class CategoryHierarchyBuilderNode(CategoryId categoryId, string title)
    {
        public CategoryId CategoryId { get; } = categoryId;
        public string Title { get; } = title;
        public List<CategoryHierarchyBuilderNode> Children { get; } = [];
    }
}
