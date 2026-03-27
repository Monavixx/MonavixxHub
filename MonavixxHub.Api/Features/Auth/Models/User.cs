using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Common.Exceptions;
using MonavixxHub.Api.Features.Auth.Exceptions;

namespace MonavixxHub.Api.Features.Auth.Models;

public class User
{
    public const int EmailMaxLength = 254;
    public const int UsernameMaxLength = 150;
    public const int UsernameMinLength = 3;
    public const int PasswordMinLength = 8;
    [Required] public int Id { get; set; }
    [UniqueDomainException(typeof(UserWithSuchUsernameAlreadyExistsException))]
    [Required] public string Username { get; set; }
    [Required] public byte[] PasswordHash { get; set; }
    [UniqueDomainException(typeof(UserWithSuchEmailAlreadyExistsException))]
    [Required] public string Email { get; set; }
    [Required] public DateTimeOffset CreatedAt { get; set; }
}