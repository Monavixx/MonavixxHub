using System.ComponentModel.DataAnnotations;

namespace MonavixxHub.Api.Common.Options;

public class EmailOptions
{
    public const string Name = "Email";
    [Required] public string Host { get; set; }
    [Required] public int Port { get; set; }
    [Required] public string Username { get; set; }
    [Required] public string Password { get; set; }
    [Required] public string From { get; set; }
    [Required] public string ConfirmationUrl { get; set; }
}