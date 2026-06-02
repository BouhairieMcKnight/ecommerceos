namespace ECommerceOS.Shared.DTOs.Command;

public interface IIdempotentCommand
{
    public Guid IdempotentCommandId { get; init; }
}