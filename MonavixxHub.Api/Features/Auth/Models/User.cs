namespace MonavixxHub.Api.Features.Auth.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public byte[] PasswordHash { get; set; }
    public string Email { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}