namespace MonavixxHub.Api.Features.Auth.DTOs;

/// <summary>
/// Represents the response returned after successful authentication.
/// </summary>
/// <param name="Token">
/// The authentication token used for subsequent requests.
/// </param>
/// <param name="Username">
/// The username of the authenticated user.
/// </param>
public record AuthResponseDto(string Token, string Username);