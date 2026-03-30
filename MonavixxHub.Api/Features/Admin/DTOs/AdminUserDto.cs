using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using MonavixxHub.Api.Features.Auth.Models;

namespace MonavixxHub.Api.Features.Admin.DTOs;

public record AdminUserDto(
    UserIdType Id,
    string Username,
    byte[] PasswordHash,
    string Email,
    DateTimeOffset CreatedAt,
    UserRole Role
)
{
    public static AdminUserDto From(User user) 
        => new(user.Id, user.Username, user.PasswordHash, user.Email, user.CreatedAt, user.Role);

    public static Expression<Func<User, AdminUserDto>> Projection
        => user => new AdminUserDto(user.Id, user.Username, user.PasswordHash, user.Email, user.CreatedAt, user.Role);
}