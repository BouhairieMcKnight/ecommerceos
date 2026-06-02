namespace ECommerceOS.Shared.Result;

public sealed record Error
{
    private Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    public string Code { get; }
    public string Description { get; }
    public ErrorType Type { get; }

    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);
    
    public static Error Validation(string code, string description) =>
        new(code, description, ErrorType.Validation);
    
    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);
    public static Error Failure(string code, string description) =>
        new(code, description, ErrorType.Failure);
}
public enum ErrorType
{
    Failure = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3
}