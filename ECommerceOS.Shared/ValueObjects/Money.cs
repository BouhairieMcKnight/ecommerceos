namespace ECommerceOS.Shared.ValueObjects;

public sealed record Money
{
    private Money(Currency currency, decimal amount)
    {
        CurrencyValue = currency;
        Amount = amount;
    }

    public decimal Amount { get; set; }
    
    public Currency CurrencyValue { get; }

    public static Money? Create(Currency currency, decimal amount)
    {
        return amount < 0 ? null : new Money(currency, amount);
    }

    public override string ToString()
    {
        return $"{Amount}_{CurrencyValue.Code}_{CurrencyValue.Symbol}_{CurrencyValue.DecimalPlaces}";
    }

    public static Money Create(string moneyValue)
    {
        var parts = moneyValue.Split('_');
        
        var amount = decimal.Parse(parts[0]);
        var currency = Currency.Create(parts[1]);
        
        return Money.Create(currency, amount)!;
    }

    public static Money? Create(string currencyString, decimal amount)
    {
        if (string.IsNullOrEmpty(currencyString) || amount < 0)
        {
            return null;
        }
        
        var currency = Currency.Create(currencyString);
        
        return new Money(currency, amount);
    }
    
    public static Money operator+(Money a, Money b)
    {
        var sum = a.Amount + b.Amount;
        a.Amount = sum;
        
        return a;
    }
}