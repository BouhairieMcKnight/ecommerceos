namespace ECommerceOS.Shared.ValueObjects;

public record Currency
{
    private Currency()
    {
    }

    public string Code { get; init; } = string.Empty;
    public string Symbol { get; init; } = string.Empty;
    public short DecimalPlaces { get; init; }

    public static readonly Currency Usd = new() { Code = "USD", Symbol = "$", DecimalPlaces = 2};
    public static readonly Currency Eur = new() { Code = "EUR", Symbol = "€", DecimalPlaces = 2};
    public static Currency Create(string code) => new() { Code = code, Symbol = "", DecimalPlaces = 2 };
}