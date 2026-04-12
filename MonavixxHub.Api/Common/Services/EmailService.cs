using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MonavixxHub.Api.Common.Options;

namespace MonavixxHub.Api.Common.Services;

public class EmailService (IOptions<EmailOptions> options) : IEmailService
{
    private readonly EmailOptions _options = options.Value;
    public async Task SendConfirmationAsync(string email, string token)
    {
        await SendHtmlAsync(email, "Confirm your email",
            $"""
             <h1>Email confirmation</h1>
             <p>Click the link below to confirm your email:</p>
             <a href="{_options.ConfirmationUrl}?token={Uri.EscapeDataString(token)}">Confirm email</a>
             """);
    }

    public async Task SendHtmlAsync(string email, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("MonavixxHub", _options.From));
        message.To.Add(new MailboxAddress("", email));
        message.Subject = "Confirm your email";
        message.Body = new TextPart("html")
        {
            Text = body
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_options.Username, _options.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}