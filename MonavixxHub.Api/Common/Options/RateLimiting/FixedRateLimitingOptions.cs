using System.ComponentModel.DataAnnotations;

namespace MonavixxHub.Api.Common.Options.RateLimiting;

public class FixedRateLimitingOptions
{
    [Required] public int PermitLimit { get; set; }
    [Required] public int WindowSeconds { get; set; }
    [Required] public int QueueLimit { get; set; }
}