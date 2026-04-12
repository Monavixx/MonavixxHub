namespace MonavixxHub.Api.Common.Services;

/// <summary>
/// Provides email sending functionality.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends a confirmation email to the specified recipient.
    /// </summary>
    /// <param name="email">The recipient's email address.</param>
    /// <param name="token">The email confirmation token.</param>
    Task SendConfirmationAsync(string email, string token);

    /// <summary>
    /// Sends an HTML email to the specified recipient.
    /// </summary>
    /// <param name="email">The recipient's email address.</param>
    /// <param name="subject">The email subject.</param>
    /// <param name="body">The HTML email body.</param>
    Task SendHtmlAsync(string email, string subject, string body);
}

