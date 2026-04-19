using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using MonavixxHub.Api.Features.Auth.Models;
using MonavixxHub.Api.Features.Auth.Services;
using Xunit;

namespace MonavixxHub.Api.UnitTests.Features.Auth.Services;

public class JwtTokenServiceTests
{
    [Fact]
    public void GenerateToken_ReturnsNonEmptyTokenAndExpiryInFuture()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "super-secret-kHUIUIHGRUIHIHUFHERBVUIRBIGHRIUPey-1234567890",
                ["Jwt:Issuer"] = "monavixx",
                ["Jwt:Audience"] = "monavixx-audience",
                ["Jwt:ExpiryMinutes"] = "60"
            })
            .Build();

        var service = new JwtTokenService(config);
        var user = new User
        {
            Id = 1,
            Username = "alice",
            Email = "alice@example.com",
            PasswordHash = Array.Empty<byte>(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        var (token, expires) = service.GenerateToken(user);

        token.Should().NotBeNullOrWhiteSpace();
        expires.Should().BeAfter(DateTime.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public void Validate_ReturnsPrincipalWithExpectedClaimsForValidToken()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "another-verO)#IR9f03j)(()U)(JoiFFsfdwferfy-secret-key-000",
                ["Jwt:Issuer"] = "monavixx",
                ["Jwt:Audience"] = "monavixx-audience",
                ["Jwt:ExpiryMinutes"] = "30"
            })
            .Build();

        var service = new JwtTokenService(config);
        var user = new User
        {
            Id = 42,
            Username = "bob",
            Email = "bob@example.com",
            PasswordHash = Array.Empty<byte>(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        var (token, _) = service.GenerateToken(user);
        var principal = service.Validate(token);

        principal.Should().NotBeNull();
        principal!.Identity.Should().NotBeNull();
        principal.Identity!.IsAuthenticated.Should().BeTrue();
        principal.FindFirst(System.Security.Claims.ClaimTypes.Name)!.Value.Should().Be(user.Username);
        principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value.Should().Be(user.Id.ToString());
        principal.FindFirst(System.Security.Claims.ClaimTypes.Email)!.Value.Should().Be(user.Email);
    }

    [Fact]
    public void Validate_ReturnsNullForTamperedToken()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "tampeP_fpepfg-wpog-_OIF)(*Y$jimvnvcxknvka;fe59r-key-123456789",
                ["Jwt:Issuer"] = "monavixx",
                ["Jwt:Audience"] = "monavixx-audience",
                ["Jwt:ExpiryMinutes"] = "15"
            })
            .Build();

        var service = new JwtTokenService(config);
        var user = new User
        {
            Id = 7,
            Username = "charlie",
            Email = "charlie@example.com",
            PasswordHash = Array.Empty<byte>(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        var (token, _) = service.GenerateToken(user);
        var tampered = token.Length > 10
            ? token.Substring(0, token.Length - 10) + new string('x', 10)
            : token + "x";

        var principal = service.Validate(tampered);

        principal.Should().BeNull();
    }

    [Fact]
    public void Validate_ReturnsNullForExpiredToken()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "expired-kePPfeofk0494i8(*U8U*(Y(**(YU*Y*(Y*&Y*(Yy-000",
                ["Jwt:Issuer"] = "monavixx",
                ["Jwt:Audience"] = "monavixx-audience",
                ["Jwt:ExpiryMinutes"] = "-5"
            })
            .Build();

        var service = new JwtTokenService(config);
        var user = new User
        {
            Id = 99,
            Username = "dave",
            Email = "dave@example.com",
            PasswordHash = Array.Empty<byte>(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        var (token, expires) = service.GenerateToken(user);
        expires.Should().BeBefore(DateTime.UtcNow);

        var principal = service.Validate(token);

        principal.Should().BeNull();
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenJwtKeyIsMissing()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "monavixx",
                ["Jwt:Audience"] = "monavixx-audience",
                ["Jwt:ExpiryMinutes"] = "30"
            })
            .Build();

        Action act = () => _ = new JwtTokenService(config);

        act.Should().Throw<ArgumentNullException>();
    }
}

