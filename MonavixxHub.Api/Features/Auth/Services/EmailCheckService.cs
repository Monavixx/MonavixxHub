using System.Text.RegularExpressions;

namespace MonavixxHub.Api.Features.Auth.Services;

/// <summary>
/// Validates whether a string is a valid email address.
/// </summary>
public partial class EmailCheckService
{
    private static readonly Regex EmailRegex = MyRegex();
    /// <summary>
    /// Determines whether the specified string is a valid email address.
    /// </summary>
    /// <param name="email">A string to check.</param>
    /// <returns>true if <paramref name="email"/> is a valid email address.</returns>
    public bool IsValid(string email) => EmailRegex.IsMatch(email);
    [GeneratedRegex(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")]
    private static partial Regex MyRegex();
}