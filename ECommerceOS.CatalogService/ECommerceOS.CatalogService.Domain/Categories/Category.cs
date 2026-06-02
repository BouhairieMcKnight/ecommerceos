namespace ECommerceOS.CatalogService.Domain.Categories;

public class Category : AggregateRoot<CategoryId>, IAuditableEntity
{
    public string Title { get; private set; } = null!;
    public CategoryId? ParentCategoryId { get; private set; }
    public Category? ParentCategory { get; private set; }
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset ModifiedOn { get; set; }
    private readonly HashSet<Category> _childrenCategories = [];
    public IEnumerable<Category> ChildrenCategories => _childrenCategories.AsReadOnly();

    private Category()
    {
    }

    public static Result<Category> Create(string title, CategoryId? parentCategoryId = null, CategoryId? categoryId = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<Category>.Failure(CategoryErrors.NotValidCategory);
        }
        
        categoryId ??= new CategoryId(Guid.NewGuid());
        var category = new Category
        {
            Id = categoryId,
            Title = title,
            ParentCategoryId = parentCategoryId
        };
        
        return Result<Category>.Success(category);
    }
    
    public Result<Category> Delete()
    {
        AddDomainEvent(new CategoryDeletedDomainEvent(Id, DateTimeOffset.Now));
        return Result<Category>.Success(this);
    }
    
    public Result<Category> Rename(string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
        {
            return Result<Category>.Failure(CategoryErrors.NotValidCategory);
        }
        
        Title = newTitle;
        return Result<Category>.Success(this);
    }

    public Result<Category> MoveTo(CategoryId newParentCategoryId)
    {
        if (newParentCategoryId == Id || _childrenCategories.Any(c => c.Id == newParentCategoryId))
        {
            return Result<Category>.Failure(CategoryErrors.NotValidCategory);
        }
        
        ParentCategoryId = newParentCategoryId;
        return Result<Category>.Success(this);
    }
}