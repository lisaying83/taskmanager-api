using FluentAssertions;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Domain.Tests.Entities;

public class UserTests
{
    [Fact]
    public void Create_WithValidData_NormalizesEmail()
    {
        var user = User.Create("TEST@EXAMPLE.COM", "Alice", "hash");

        user.Email.Should().Be("test@example.com");
        user.Name.Should().Be("Alice");
    }

    [Fact]
    public void Create_WithEmptyEmail_ThrowsDomainException()
    {
        var act = () => User.Create("", "Alice", "hash");

        act.Should().Throw<DomainException>().WithMessage("*Email*");
    }

    [Fact]
    public void Create_WithEmptyName_ThrowsDomainException()
    {
        var act = () => User.Create("alice@example.com", "", "hash");

        act.Should().Throw<DomainException>().WithMessage("*Name*");
    }

    [Fact]
    public void SetRefreshToken_StoresTokenAndExpiry()
    {
        var user = User.Create("alice@example.com", "Alice", "hash");
        var expiry = DateTime.UtcNow.AddDays(7);

        user.SetRefreshToken("token123", expiry);

        user.RefreshToken.Should().Be("token123");
        user.RefreshTokenExpiresAt.Should().Be(expiry);
    }

    [Fact]
    public void HasValidRefreshToken_WithValidToken_ReturnsTrue()
    {
        var user = User.Create("alice@example.com", "Alice", "hash");
        user.SetRefreshToken("token123", DateTime.UtcNow.AddDays(7));

        user.HasValidRefreshToken("token123").Should().BeTrue();
    }

    [Fact]
    public void HasValidRefreshToken_WithExpiredToken_ReturnsFalse()
    {
        var user = User.Create("alice@example.com", "Alice", "hash");
        user.SetRefreshToken("token123", DateTime.UtcNow.AddSeconds(-1));

        user.HasValidRefreshToken("token123").Should().BeFalse();
    }

    [Fact]
    public void RevokeRefreshToken_ClearsToken()
    {
        var user = User.Create("alice@example.com", "Alice", "hash");
        user.SetRefreshToken("token123", DateTime.UtcNow.AddDays(7));

        user.RevokeRefreshToken();

        user.RefreshToken.Should().BeNull();
        user.RefreshTokenExpiresAt.Should().BeNull();
    }
}
