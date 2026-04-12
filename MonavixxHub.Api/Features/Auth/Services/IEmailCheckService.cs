namespace MonavixxHub.Api.Features.Auth.Services;

/// <summary>
/// Validates whether a string is a valid email address.
/// </summary>
public interface IEmailCheckService
{
    /// <summary>
    /// Determines whether the specified string is a valid email address.
    /// </summary>
    /// <param name="email">A string to check.</param>
    /// <returns>true if <paramref name="email"/> is a valid email address.</returns>
    bool IsValid(string email);
}

