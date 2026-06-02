namespace ECommerceOS.AuthService.Domain.Identity;

public static class IdentityErrors
{
    public static readonly Error CustomerNotFound = Error.NotFound("Customer.NotFound",
        "Customer not found");
    
    public static readonly Error NotValidCustomer = Error.Validation("Customer.Validation",
        "Customer is not valid");

    public static Error NotFoundByCustomerId(UserId customerId) => Error.NotFound("Customer.NotFoundByCustomerId",
        $"No customer found associated with {customerId}");
    
    public static readonly Error SellerNotFound = Error.NotFound("Seller.NotFound", "Seller not found");
    
    public static readonly Error NotValidSeller = Error.Validation("Seller.Validation",
        "Seller is not valid");
}