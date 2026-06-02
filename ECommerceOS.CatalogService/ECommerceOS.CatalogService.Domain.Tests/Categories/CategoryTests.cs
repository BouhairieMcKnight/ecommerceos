using ECommerceOS.CatalogService.Domain.Categories;

namespace ECommerceOS.CatalogService.Domain.Tests.Categories;

public sealed class CategoryTests
{
    [Fact]
    public void Create_WithWhitespaceTitle_ReturnsFailure()
    {
        var result = Category.Create(" ");

        Assert.False(result.IsSuccess);
        Assert.Equal(CategoryErrors.NotValidCategory, result.Error);
        Assert.Null(result.Value);
    }

    [Fact]
    public void Rename_WithValidTitle_UpdatesCategoryTitle()
    {
        var createResult = Category.Create("Old Title");
        var category = Assert.IsType<Category>(createResult.Value);

        var result = category.Rename("New Title");

        Assert.True(result.IsSuccess);
        Assert.Equal("New Title", category.Title);
    }

    [Fact]
    public void MoveTo_WithSameCategoryId_ReturnsFailure()
    {
        var categoryId = new CategoryId(Guid.NewGuid());
        var createResult = Category.Create("Books", categoryId: categoryId);
        var category = Assert.IsType<Category>(createResult.Value);

        var result = category.MoveTo(categoryId);

        Assert.False(result.IsSuccess);
        Assert.Equal(CategoryErrors.NotValidCategory, result.Error);
    }
}
