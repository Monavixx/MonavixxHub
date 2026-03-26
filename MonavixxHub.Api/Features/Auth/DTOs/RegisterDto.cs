using System.ComponentModel.DataAnnotations;
using MonavixxHub.Api.Features.Auth.Models;

namespace MonavixxHub.Api.Features.Auth.DTOs;

public record RegisterDto
(
    [StringLength(User.UsernameMaxLength, MinimumLength = User.UsernameMinLength)]
    string Username,
    [StringLength(User.EmailMaxLength)]
    string Email,
    string Password
);