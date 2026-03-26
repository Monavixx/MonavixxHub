using System.ComponentModel.DataAnnotations;

namespace MonavixxHub.Api.Features.Auth.DTOs;

public record LoginDto
(
    [Required] string UsernameOrEmail,
    [Required] string Password
);