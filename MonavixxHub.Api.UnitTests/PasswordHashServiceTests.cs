using MonavixxHub.Api.Features.Auth.Services;

namespace MonavixxHub.Api.UnitTests;

public class PasswordHashServiceTests
{
    [Fact]
    public void Hash_ShouldCreateDifferentHashesForTheSamePassword()
    {
        var service = new PasswordHashService();
        const string password = "abcdefghijklmnop";
        var hash1 = service.Hash(password);
        var hash2 = service.Hash(password);
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void Verify_WhenCorrectPassword_ShouldReturnTrue()
    {
        var service = new PasswordHashService();
        const string password = "hthtr48gr7tdghre&Y&Y^834r8";
        var hash = service.Hash(password);
        Assert.True(service.Verify(password, hash));
    }
    
    [Fact]
    public void Verify_WhenIncorrectPassword_ShouldReturnFalse()
    {
        var service = new PasswordHashService();
        const string password = "HhhHu89((93366";
        var hash = service.Hash(password);
        Assert.False(service.Verify("9i9kaaa000", hash));
    }
}