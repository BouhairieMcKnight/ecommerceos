namespace ECommerceOS.OrderService.Domain.Orders;

public enum OrderStatus
{
    None = 0,
    Pending = 1 << 1,
    Processing  = 1 << 2,
    Delivering = 1 << 3,
    Delivered = 1 << 4,
    Cancelled = 1 << 5
}

public static class OrderStatusExtensions
{
    public static int GetLatest(this OrderStatus status)
    {
        var value = (int)status;
        var setValues = Enum.GetValues(typeof(OrderStatus)).
            Cast<int>().Where(f => (f & value) == f).ToArray();
        return setValues.Any() ? setValues.Max() : 0;
    }
}