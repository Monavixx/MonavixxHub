using System.Security.Cryptography;
using System.Text;
using MonavixxHub.Api.Features.Auth.Models;

namespace MonavixxHub.Api.Features.Auth.Services;

public class RefreshTokenService(IConfiguration configuration)
{
    public byte[] NewRefreshToken => RandomNumberGenerator.GetBytes(64);
    public DateTimeOffset Expiry => DateTimeOffset.UtcNow + Session.Expiration;

    public byte[] Hash(string refreshToken) => Hash(Convert.FromBase64String(refreshToken));
    public byte[] Hash(byte[] refreshToken)
        => SHA256.HashData(refreshToken.Concat(Encoding.UTF8.GetBytes(configuration["RefreshToken:Key"]!))
            .ToArray());
    public string RefreshTokenToString(byte[] refreshToken) => Convert.ToBase64String(refreshToken);
    public byte[] RefreshTokenFromString(string refreshToken) => Convert.FromBase64String(refreshToken);

    public bool Verify(string refreshToken, byte[] hash)
        => Hash(Convert.FromBase64String(refreshToken)).SequenceEqual(hash);
}