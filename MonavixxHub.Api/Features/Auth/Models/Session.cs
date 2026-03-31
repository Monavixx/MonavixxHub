using System.ComponentModel.DataAnnotations;
using System.Net;

namespace MonavixxHub.Api.Features.Auth.Models;

public class Session
{
    public static readonly TimeSpan Expiration = new (31, 0, 0, 0);
    [Required] public UserIdType UserId { get; set; }
    [Required] public byte[] RefreshTokenHash { get; set; }
    [Required] public DateTimeOffset ExpiresAt { get; set; }
    [Required] public Guid Id { get; set; } = Guid.NewGuid();
    public IPAddress? Ip { get; set; }
    public User User { get; set; }
}