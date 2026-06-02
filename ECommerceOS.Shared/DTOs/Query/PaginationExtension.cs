using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ECommerceOS.Shared.DTOs.Query;

public static class PaginationExtension
{
    public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(
        this IQueryable<TDestination> queryable,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
        where TDestination : class
        => PaginatedList<TDestination>.CreateAsync(
            queryable.AsNoTracking(), pageNumber, pageSize, cancellationToken);

    public static Task<List<TDestination>> ProjectToListAsync<TDestination>(
        this IQueryable queryable,
        CancellationToken cancellationToken = default)
        where TDestination : class
        => queryable.ProjectToType<TDestination>().AsNoTracking().ToListAsync(cancellationToken);
}