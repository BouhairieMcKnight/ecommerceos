namespace ECommerceOS.OrderService.Application.Common.Interfaces;

public interface ILocationService 
{
    Task<Result> ValidateAddressAsync(Address address);
}