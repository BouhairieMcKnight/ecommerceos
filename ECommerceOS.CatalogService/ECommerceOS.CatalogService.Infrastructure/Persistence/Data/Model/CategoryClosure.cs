namespace ECommerceOS.CatalogService.Infrastructure.Persistence.Data.Model;

public class CategoryClosure 
{
    public Category ParentCategory { get; private set; }
    public Category ChildCategory { get; private set; }
    public CategoryId ParentCategoryId { get; private set; }
    public CategoryId ChildCategoryId { get; private set; }
    public int Depth { get; private set; }

    private CategoryClosure()
    {
    }

    public static CategoryClosure Create(
        CategoryId parentCategoryId,
        CategoryId childCategoryId,
        int depth)
    {
        var categoryClosure = new CategoryClosure
        {
            ParentCategoryId = parentCategoryId,
            ChildCategoryId = childCategoryId,
            Depth = depth
        };

        return categoryClosure;
    }
}