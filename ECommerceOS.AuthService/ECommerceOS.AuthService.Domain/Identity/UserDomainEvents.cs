namespace ECommerceOS.AuthService.Domain.Identity;


public record UserRegisteredDomainEvent : IDomainEvent
{
    public string Type => nameof(User);
    public string Email { get; private set; } = string.Empty;
    public string? Password { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Role Role { get; private set; }
    public UserId UserId { get; private set; }
    public DateTimeOffset OccurredOn { get; init; }
    private UserRegisteredDomainEvent()
    {
    }
    
    public static UserRegisteredDomainEvent Create(
        string email,
        UserId userId,
        string name,
        Role role,
        string? password = null)
    {
        var userRegisteredEvent = new UserRegisteredDomainEvent
        {
            Name = name,
            Role = role,
            Password = password,
            Email = email,
            UserId = userId,
            OccurredOn = DateTimeOffset.Now,
        };
        
        return userRegisteredEvent;
    }
};

public record CreatedRefreshTokenDomainEvent: IDomainEvent
{
    public string Type => nameof(User);
    public string Email { get; private set; } = string.Empty;
    public UserId UserId { get; private set; }
    public DateTimeOffset OccurredOn { get; init; }
    public string RefreshTokenId { get; private set; }
    private CreatedRefreshTokenDomainEvent()
    {
    }
    
    public static CreatedRefreshTokenDomainEvent Create(string email, UserId userId, string refreshTokenId)
    {
        var createdRefreshTokenDomainEvent = new CreatedRefreshTokenDomainEvent
        {
            Email = email,
            UserId = userId,
            RefreshTokenId =  refreshTokenId,
            OccurredOn= DateTimeOffset.Now,
        };
        
        return createdRefreshTokenDomainEvent;
    }
}

public record RevokedRefreshTokenDomainEvent : IDomainEvent
{
    public string Type => nameof(User);
    public UserId UserId { get; private set; }
    public DateTimeOffset OccurredOn { get; init; }
    public string RefreshTokenId { get; private set; }
    
    private RevokedRefreshTokenDomainEvent()
    {
    }
    
    public static RevokedRefreshTokenDomainEvent Create(UserId userId, string refreshTokenId)
    {
        var revokedRefreshTokenDomainEvent= new RevokedRefreshTokenDomainEvent
        {
            UserId = userId,
            RefreshTokenId =  refreshTokenId,
            OccurredOn= DateTimeOffset.Now,
        };
        
        return revokedRefreshTokenDomainEvent;
    } 
}

public record UserVerifiedDomainEvent : IDomainEvent
{
    public string Type => nameof(User);
    public UserId UserId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public DateTimeOffset OccurredOn { get; init; }

    private UserVerifiedDomainEvent()
    {
    }

    public static UserVerifiedDomainEvent Create(UserId userId, string email, string name)
    {
        var userVerifiedDomainEvent = new UserVerifiedDomainEvent
        {
            UserId = userId,
            Email = email,
            Name = name,
            OccurredOn = DateTimeOffset.UtcNow
        };

        return userVerifiedDomainEvent;
    }
}

public record ClearRefreshTokensDomainEvent : IDomainEvent
{
    public string Type => nameof(User);
    public UserId UserId { get; private set; }
    public DateTimeOffset OccurredOn { get; init; }
    private ClearRefreshTokensDomainEvent()
    {
    }
    
    public static ClearRefreshTokensDomainEvent Create(UserId userId)
    {
        var clearRefreshTokensDomainEvent = new ClearRefreshTokensDomainEvent
        {
            UserId = userId,
            OccurredOn= DateTimeOffset.Now,
        };
        
        return clearRefreshTokensDomainEvent;
    } 
}
