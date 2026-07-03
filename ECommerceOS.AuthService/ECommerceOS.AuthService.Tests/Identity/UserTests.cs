using ECommerceOS.AuthService.Domain.Identity;
using ECommerceOS.Shared.ValueObjects;
using Xunit;

namespace ECommerceOS.AuthService.Tests.Identity;

public class UserTests
{
    [Fact]
    public void Create_ShouldInitializeUser_AndAddRegisteredEvent()
    {
        var userId = new UserId(Guid.NewGuid());

        var result = User.Create(
            userName: "Alice",
            role: Role.Customer,
            email: "alice@example.com",
            password: "Password!1",
            customerId: userId);

        Assert.True(result.IsSuccess);
        var user = Assert.IsType<User>(result.Value);
        Assert.Equal(userId, user.Id);
        Assert.Equal("Alice", user.Name);
        Assert.Equal("alice@example.com", user.Email);
        Assert.Equal(Role.Customer, user.Role);
        Assert.False(user.IsEmailVerified);
        Assert.Contains(user.DomainEvents, e => e is UserRegisteredDomainEvent);
    }

    [Fact]
    public void Create_WhenEmailAlreadyVerified_ShouldAddVerifiedEvent()
    {
        var result = User.Create(
            userName: "Alice",
            role: Role.Customer,
            email: "alice@example.com",
            isEmailVerified: true);

        var user = Assert.IsType<User>(result.Value);

        Assert.True(user.IsEmailVerified);
        Assert.Contains(user.DomainEvents, e => e is UserRegisteredDomainEvent);
        Assert.Contains(user.DomainEvents, e => e is UserVerifiedDomainEvent);
    }

    [Fact]
    public void Create_WhenPasswordIsEmptyString_ShouldNormalizePasswordToNull()
    {
        var result = User.Create(
            userName: "Alice",
            role: Role.Customer,
            email: "alice@example.com",
            password: string.Empty);

        var user = Assert.IsType<User>(result.Value);

        Assert.Null(user.Password);
    }

    [Fact]
    public void VerifyEmail_WhenNotVerified_ShouldSetFlagAndAddDomainEvent()
    {
        var result = User.Create("Alice", Role.Customer, "alice@example.com");
        var user = Assert.IsType<User>(result.Value);
        user.ClearDomainEvents();

        var verifyResult = user.VerifyEmail();

        Assert.True(verifyResult.IsSuccess);
        Assert.True(user.IsEmailVerified);
        Assert.Contains(user.DomainEvents, e => e is UserVerifiedDomainEvent);
    }

    [Fact]
    public void VerifyEmail_WhenAlreadyVerified_ShouldNotAddDuplicateDomainEvent()
    {
        var result = User.Create(
            userName: "Alice",
            role: Role.Customer,
            email: "alice@example.com",
            isEmailVerified: true);
        var user = Assert.IsType<User>(result.Value);
        user.ClearDomainEvents();

        var verifyResult = user.VerifyEmail();

        Assert.True(verifyResult.IsSuccess);
        Assert.Empty(user.DomainEvents);
    }

    [Fact]
    public void CreateRefreshToken_ShouldStoreToken_AndAddCreatedTokenEvent()
    {
        var userResult = User.Create("Alice", Role.Customer, "alice@example.com");
        var user = Assert.IsType<User>(userResult.Value);
        user.ClearDomainEvents();

        var createTokenResult = user.CreateRefreshToken("token-1");

        Assert.True(createTokenResult.IsSuccess);
        Assert.Contains(user.RefreshTokens, t => t.Token == "token-1");
        Assert.Contains(user.DomainEvents, e => e is CreatedRefreshTokenDomainEvent);
    }

    [Fact]
    public void RevokeRefreshToken_WhenTokenExists_ShouldMarkTokenRevoked_AndAddRevokedEvent()
    {
        var userResult = User.Create("Alice", Role.Customer, "alice@example.com");
        var user = Assert.IsType<User>(userResult.Value);
        user.CreateRefreshToken("token-1");
        user.ClearDomainEvents();

        var revokeResult = user.RevokeRefreshToken("token-1");

        Assert.True(revokeResult.IsSuccess);
        Assert.Contains(user.RefreshTokens, t => t.Token == "token-1" && t.IsRevoked);
        Assert.Contains(user.DomainEvents, e => e is RevokedRefreshTokenDomainEvent);
    }

    [Fact]
    public void RotateRefreshToken_WhenTokenExists_ShouldRevokeCurrentToken_AndCreateNextInSameFamily()
    {
        var userResult = User.Create("Alice", Role.Customer, "alice@example.com");
        var user = Assert.IsType<User>(userResult.Value);
        var createTokenResult = user.CreateRefreshToken("token-1");
        var familyId = createTokenResult.Value!.FamilyId;
        user.ClearDomainEvents();

        var rotateResult = user.RotateRefreshToken("token-1", "token-2");

        Assert.True(rotateResult.IsSuccess);
        Assert.Contains(user.RefreshTokens, t => t.Token == "token-1" && t.IsRevoked);
        Assert.Contains(user.RefreshTokens, t => t.Token == "token-2" && t.FamilyId == familyId && !t.IsRevoked);
        Assert.Contains(user.DomainEvents, e => e is RevokedRefreshTokenDomainEvent);
        Assert.Contains(user.DomainEvents, e => e is CreatedRefreshTokenDomainEvent);
    }

    [Fact]
    public void RotateRefreshToken_WhenRevokedTokenIsReused_ShouldDeleteTokenFamily()
    {
        var userResult = User.Create("Alice", Role.Customer, "alice@example.com");
        var user = Assert.IsType<User>(userResult.Value);
        user.CreateRefreshToken("token-1");
        user.RotateRefreshToken("token-1", "token-2");
        user.ClearDomainEvents();

        var rotateResult = user.RotateRefreshToken("token-1", "token-3");

        Assert.False(rotateResult.IsSuccess);
        Assert.Empty(user.RefreshTokens);
        Assert.Contains(user.DomainEvents, e => e is DeletedRefreshTokenFamilyDomainEvent);
    }

    [Fact]
    public void DeleteRefreshTokenFamily_WhenTokenExists_ShouldRemoveOnlyThatFamily()
    {
        var userResult = User.Create("Alice", Role.Customer, "alice@example.com");
        var user = Assert.IsType<User>(userResult.Value);
        user.CreateRefreshToken("family-1-token-1");
        user.RotateRefreshToken("family-1-token-1", "family-1-token-2");
        user.CreateRefreshToken("family-2-token-1");
        user.ClearDomainEvents();

        var deleteResult = user.DeleteRefreshTokenFamily("family-1-token-2");

        Assert.True(deleteResult.IsSuccess);
        Assert.DoesNotContain(user.RefreshTokens, t => t.Token.StartsWith("family-1", StringComparison.Ordinal));
        Assert.Contains(user.RefreshTokens, t => t.Token == "family-2-token-1");
        Assert.Contains(user.DomainEvents, e => e is DeletedRefreshTokenFamilyDomainEvent);
    }

    [Fact]
    public void RevokeRefreshToken_WhenTokenDoesNotExist_ShouldReturnFailure()
    {
        var userResult = User.Create("Alice", Role.Customer, "alice@example.com");
        var user = Assert.IsType<User>(userResult.Value);

        var revokeResult = user.RevokeRefreshToken("missing-token");

        Assert.False(revokeResult.IsSuccess);
        Assert.Equal("RevokeRefreshToken", revokeResult.Error?.Code);
    }

    [Fact]
    public void ClearRefreshTokens_ShouldRemoveAllTokens_AndAddDomainEvent()
    {
        var userResult = User.Create("Alice", Role.Customer, "alice@example.com");
        var user = Assert.IsType<User>(userResult.Value);
        user.CreateRefreshToken("token-1");
        user.CreateRefreshToken("token-2");
        user.ClearDomainEvents();

        var clearResult = user.ClearRefreshTokens();

        Assert.True(clearResult.IsSuccess);
        Assert.Empty(user.RefreshTokens);
        Assert.Contains(user.DomainEvents, e => e is ClearRefreshTokensDomainEvent);
    }
}
