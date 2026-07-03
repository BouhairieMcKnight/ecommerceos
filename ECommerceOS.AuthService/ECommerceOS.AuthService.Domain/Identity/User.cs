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
        
        AddDomainEvent(RevokedRefreshTokenDomainEvent.Create(Id, tokenId));
        return Result<User>.Success(this);
    }

    public Result<User> DeleteRefreshTokenFamily(string tokenId)
    {
        var refreshToken = _refreshTokens.FirstOrDefault(t => t.Token == tokenId);

        if (refreshToken is null)
        {
            return Result<User>
                .Failure(Error.Conflict("DeleteRefreshTokenFamily", "Could not delete refresh token family"));
        }

        DeleteRefreshTokenFamilyById(refreshToken.FamilyId);
        return Result<User>.Success(this);
    }

    public Result<RefreshToken> RotateRefreshToken(string currentTokenId, string nextTokenId)
    {
        RemoveExpiredRefreshTokenFamilies();

        var currentRefreshToken = _refreshTokens.FirstOrDefault(t => t.Token == currentTokenId);

        if (currentRefreshToken is null)
        {
            return Result<RefreshToken>
                .Failure(Error.Conflict("RefreshToken", "Could not rotate refresh token"));
        }

        if (currentRefreshToken is { IsRevoked: true } or { IsExpired: true })
        {
            DeleteRefreshTokenFamilyById(currentRefreshToken.FamilyId);
            return Result<RefreshToken>.Failure(IdentityErrors.NotValidCustomer);
        }

        currentRefreshToken.Revoke();

        var nextRefreshToken = RefreshToken.Create(nextTokenId, currentRefreshToken.FamilyId);
        var result = AddRefreshToken(nextRefreshToken);

        if (!result.IsSuccess)
        {
            return result;
        }

        AddDomainEvent(RevokedRefreshTokenDomainEvent.Create(Id, currentTokenId));
        return Result<RefreshToken>.Success(nextRefreshToken);
    }

    public Result<User> ClearRefreshTokens()
    {
        _refreshTokens.Clear();
        AddDomainEvent(ClearRefreshTokensDomainEvent.Create(Id));
        return Result<User>.Success(this);
    }
    
    public Result<RefreshToken> CreateRefreshToken(string tokenId)
    {
        RemoveExpiredRefreshTokenFamilies();

        var refreshToken = RefreshToken.Create(tokenId);

        var result = AddRefreshToken(refreshToken);

        if (!result.IsSuccess)
        {
            return result;
        }
        
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

    private Result<RefreshToken> AddRefreshToken(RefreshToken refreshToken)
    {
        if (_refreshTokens.Any(token => token.Token == refreshToken.Token))
        {
            return Result<RefreshToken>
                .Failure(Error.Conflict("RefreshToken", "Could not create refresh token"));
        }

        _refreshTokens.Add(refreshToken);
        AddDomainEvent(CreatedRefreshTokenDomainEvent.Create(Email, Id, refreshToken.Token));
        return Result<RefreshToken>.Success(refreshToken);
    }

    private void RemoveExpiredRefreshTokenFamilies()
    {
        var expiredFamilyIds = _refreshTokens
            .Where(token => token.IsExpired)
            .Select(token => token.FamilyId)
            .Distinct()
            .ToArray();

        foreach (var familyId in expiredFamilyIds)
        {
            DeleteRefreshTokenFamilyById(familyId);
        }
    }

    private void DeleteRefreshTokenFamilyById(string familyId)
    {
        var familyTokens = _refreshTokens
            .Where(token => token.FamilyId == familyId)
            .ToArray();

        foreach (var familyToken in familyTokens)
        {
            _refreshTokens.Remove(familyToken);
        }

        if (familyTokens.Length > 0)
        {
            AddDomainEvent(DeletedRefreshTokenFamilyDomainEvent.Create(Id, familyId));
        }
    }
    
}
