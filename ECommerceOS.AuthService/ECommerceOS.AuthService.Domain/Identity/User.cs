using System.ComponentModel.DataAnnotations;

namespace ECommerceOS.AuthService.Domain.Identity;

public class User : AggregateRoot<UserId>, IAuditableEntity
{
    public Role Role { get; private set; }
    public string Email { get; private set; }
    public bool IsEmailVerified { get; private set; }
    [MaxLength(50)]
    public string? Password { get; private set; }
    public string Name { get; private set; }
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset ModifiedOn { get; set; } 
    private readonly HashSet<RefreshToken> _refreshTokens = [];
    public IEnumerable<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
    
    private User()
    {
    }

    public static Result<User> Create(
        string userName,
        Role role,
        string email,
        string? password = null,
        bool isEmailVerified = false,
        UserId? customerId = null)
    {
        if (string.IsNullOrEmpty(password) && password != null)
        {
            password = null;
        }
        
        customerId ??= new UserId(Guid.NewGuid());
        var user = new User
        {
            Id = customerId,
            Email = email,
            Password = password,
            Name = userName,
            Role = role,
            IsEmailVerified = isEmailVerified
        };
        
        user.AddDomainEvent(UserRegisteredDomainEvent.Create(
            email: user.Email,
            userId: user.Id,
            name: user.Name,
            role: user.Role,
            password: user.Password));

        if (user.IsEmailVerified)
        {
            user.AddDomainEvent(UserVerifiedDomainEvent.Create(user.Id, user.Email, user.Name));
        }
        
        return Result<User>.Success(user);
    }
    
    public Result<User> RevokeRefreshToken(string tokenId)
    {
        var refreshToken = _refreshTokens.FirstOrDefault(t => t.Token == tokenId);

        if (refreshToken is null)
        {
            return Result<User>
                .Failure(Error.Conflict("RevokeRefreshToken", "Could not revoke refresh token"));
        }

        refreshToken.Revoke();
        var result = _refreshTokens.Remove(refreshToken);

        if (!result)
        {
            return Result<User>
                .Failure(Error.Conflict("RevokeRefreshToken", "Could not revoke refresh token"));
        }
        
        AddDomainEvent(RevokedRefreshTokenDomainEvent.Create(Id, tokenId));
        return Result<User>.Success(this);
    }

    public Result<User> ClearRefreshTokens()
    {
        _refreshTokens.Clear();
        AddDomainEvent(ClearRefreshTokensDomainEvent.Create(Id));
        return Result<User>.Success(this);
    }
    
    public Result<RefreshToken> CreateRefreshToken(string tokenId)
    {
        var refreshToken = RefreshToken.Create(tokenId);
        
        var result = _refreshTokens.Add(refreshToken);
        
        if (!result)
        {
            return Result<RefreshToken>
                .Failure(Error.Conflict("RefreshToken", "Could not create refresh token"));
        }
        
        AddDomainEvent(CreatedRefreshTokenDomainEvent.Create(Email, Id, refreshToken.Token));
        return Result<RefreshToken>.Success(refreshToken);
    }

    public Result<User> VerifyEmail()
    {
        if (IsEmailVerified)
        {
            return Result<User>.Success(this);
        }

        IsEmailVerified = true;
        AddDomainEvent(UserVerifiedDomainEvent.Create(Id, Email, Name));
        return Result<User>.Success(this);
    }
    
}
