namespace ECommerceOS.CatalogService.Domain.Categories;

public record CategoryCreatedDomainEvent : IDomainEvent
{
    public string Type => nameof(Category);
    public required CategoryId CategoryId { get; init; }
    public CategoryId? ParentCategoryId { get; init; }
    public required string Title { get; init; }
    public DateTimeOffset OccurredOn { get; init; }
}

public record CategoryRenamedDomainEvent(
    CategoryId CategoryId,
    string OldTitle,
    string NewTitle,
    DateTimeOffset OccurredOn)
    : IDomainEvent
{
    public string Type => nameof(Category);
    public required CategoryId CategoryId { get; init; }
    public required string OldTitle { get; init; }
    public required string NewTitle { get; init; }
    public DateTimeOffset OccurredOn { get; init; }
    
};

public record CategoryDeletedDomainEvent(CategoryId CategoryId, DateTimeOffset OccurredOn) : IDomainEvent
{
    public string Type => nameof(Category);
};

public record CategoryMovedDomainEvent: IDomainEvent
{
    public string Type => nameof(Category);
    public required CategoryId CategoryId { get; init; }
    public CategoryId? ParentCategoryId { get; init; }
    public required CategoryId NewParentCategoryId { get; init; }
    public required string Title { get; init; }
    public DateTimeOffset OccurredOn { get; init; }
}