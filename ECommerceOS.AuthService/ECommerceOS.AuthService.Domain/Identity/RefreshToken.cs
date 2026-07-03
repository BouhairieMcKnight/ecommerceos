namespace ECommerceOS.AuthService.Domain.Identity;

public sealed class RefreshToken
{
    private static readonly TimeSpan DefaultLifetime = TimeSpan.FromDays(7);

    public string Token { get; private set; } = string.Empty;
    public string FamilyId { get; private set; } = string.Empty;
    public UserId UserId { get; private set; }
    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresOn;
    public bool IsRevoked { get; private set; }
    public DateTimeOffset CreateDate { get; private set; }
    public TimeSpan ExpiresIn { get; private set; }
    public DateTimeOffset ExpiresOn { get; private set; }

    private RefreshToken()
    {
    }

    public static RefreshToken Create(string tokenId)
    {
        return Create(tokenId, Guid.NewGuid().ToString("N"));
    }

    public static RefreshToken Create(string tokenId, string familyId)
    {
        var createdOn = DateTimeOffset.UtcNow;

        return new RefreshToken
        {
            Token = tokenId,
            FamilyId = familyId,
            CreateDate = createdOn,
            ExpiresIn = DefaultLifetime,
            ExpiresOn = createdOn.Add(DefaultLifetime),
            IsRevoked = false
        };
    }

    internal void Revoke()
    {
        IsRevoked = true;
    }
}
