namespace ECommerceOS.Shared.ValueObjects;

public record Sku
{
    private const int SkuLength = 15;

    private Sku(string value) => Value = value;

    public string Value { get; }

    public static Sku? Create(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        if (value.Length != SkuLength)
        {
            return null;
        }

        return new Sku(value);
    }
}