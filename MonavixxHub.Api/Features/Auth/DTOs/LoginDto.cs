using System.ComponentModel.DataAnnotations;
using MonavixxHub.Api.Features.Auth.Models;

namespace MonavixxHub.Api.Features.Auth.DTOs;

/// <summary>
/// Represents the credentials provided by a user to authenticate.
/// </summary>
/// <param name="UsernameOrEmail">
/// The user's identifier that can be either a username or an email address.
/// The actual type is determined during authentication.
/// </param>
/// <param name="Password">The user's password. It's verified during authentication.</param>
public record LoginDto
(
    [StringLength(User.UsernameOrEmailMaxLength)]
    [Required] string UsernameOrEmail,
    [MinLength(User.PasswordMinLength)]
    [Required] string Password
);