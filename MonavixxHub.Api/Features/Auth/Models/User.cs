using System.ComponentModel.DataAnnotations;

namespace MonavixxHub.Api.Features.Auth.Models;

public class User
{
    public const int EmailMaxLength = 254;
    public const int UsernameMaxLength = 150;
    public const int UsernameMinLength = 3;
    public const int PasswordMinLength = 8;
    [Required] public int Id { get; set; }
    [Required] public string Username { get; set; }
    [Required] public byte[] PasswordHash { get; set; }
    [Required] public string Email { get; set; }
    [Required] public DateTimeOffset CreatedAt { get; set; }
}