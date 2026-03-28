using System.ComponentModel.DataAnnotations;
using MonavixxHub.Api.Features.Auth.Models;

namespace MonavixxHub.Api.Features.Auth.DTOs;

/// <summary>
/// Represents the credentials provided by a user to register.
/// </summary>
public record RegisterDto
(
    [StringLength(User.UsernameMaxLength, MinimumLength = User.UsernameMinLength)]
    [Required]
    string Username,
    [StringLength(User.EmailMaxLength)]
    [Required]
    string Email,
    [MinLength(User.PasswordMinLength)]
    [Required] string Password
);