namespace ECommerceOS.AuthService.Domain.Identity;

public record RefreshToken
{
    public string Token { get; private set; } = string.Empty;
    public UserId UserId { get; init; }
    public bool IsExpired => DateTimeOffset.Now >= CreateDate + ExpiresIn;
    public bool IsRevoked { get; internal set; }
    internal void Revoke() => IsRevoked = true;
    public DateTimeOffset CreateDate { get; init; }
    public TimeSpan ExpiresIn { get; init; }

    private RefreshToken()
    {
    }

    public static RefreshToken Create(string tokenId)
    {
        return new RefreshToken
        {
            Token = tokenId,
            CreateDate = DateTimeOffset.Now,
            ExpiresIn = TimeSpan.FromDays(7),
            IsRevoked = false
        };
    }
}