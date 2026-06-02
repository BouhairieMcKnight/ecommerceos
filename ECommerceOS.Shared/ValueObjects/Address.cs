namespace ECommerceOS.Shared.ValueObjects;

public record Address(
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode)
{
    public static Address Create(string addressString)
    {
        var fields = addressString.Split(',');
        return new Address(fields[0], fields[1], fields[2], fields[3], fields[4]);
    }

    public override string ToString()
    {
        return $"{Street},{City},{State},{Country},{ZipCode}";
    }
};