namespace ECommerceOS.Shared.Entity;

public interface IAuditableEntity
{
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset ModifiedOn { get; set; }
}