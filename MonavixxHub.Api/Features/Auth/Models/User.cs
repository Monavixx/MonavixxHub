namespace MonavixxHub.Api.Features.Auth.Models;

public class User
{
    public const int EmailMaxLength = 254;
    public const int UsernameMaxLength = 150;
    public const int UsernameMinLength = 3;
    public int Id { get; set; }
    public string Username { get; set; }
    public byte[] PasswordHash { get; set; }
    public string Email { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}