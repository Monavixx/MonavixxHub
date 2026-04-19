using System;
using System.Linq;
using FluentAssertions;
using MonavixxHub.Api.Features.Auth.Services;
using Xunit;

namespace MonavixxHub.Api.UnitTests.Features.Auth.Services;

public class PasswordHashServiceTests
{
    [Fact]
    public void HashAndVerify_ReturnsTrueForCorrectPassword()
    {
        var service = new PasswordHashService();
        var password = "CorrectHorseBatteryStaple";

        var hash = service.Hash(password);

        service.Verify(password, hash).Should().BeTrue();
    }

    [Fact]
    public void Verify_ReturnsFalseForIncorrectPassword()
    {
        var service = new PasswordHashService();
        var password = "password123";
        var wrong = "password124";

        var hash = service.Hash(password);

        service.Verify(wrong, hash).Should().BeFalse();
    }

    [Fact]
    public void HashesAreUnique_ForSamePassword()
    {
        var service = new PasswordHashService();
        var password = "repeatable-password";

        var first = service.Hash(password);
        var second = service.Hash(password);

        first.SequenceEqual(second).Should().BeFalse();
    }

    [Fact]
    public void Verify_ReturnsFalseWhenHashIsTampered()
    {
        var service = new PasswordHashService();
        var password = "letmein";

        var hash = service.Hash(password);
        // tamper with first byte of the hash
        hash[0] ^= 0xFF;

        service.Verify(password, hash).Should().BeFalse();
    }

    [Fact]
    public void Hash_ThrowsArgumentNullException_WhenPasswordIsNull()
    {
        var service = new PasswordHashService();

        Action act = () => service.Hash(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Verify_ThrowsNullReferenceException_WhenHashIsNull()
    {
        var service = new PasswordHashService();

        Action act = () => service.Verify("pwd", null!);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Verify_ThrowsArgumentOutOfRangeException_WhenHashIsTooShort()
    {
        var service = new PasswordHashService();

        Action act = () => service.Verify("pwd", new byte[10]);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}

