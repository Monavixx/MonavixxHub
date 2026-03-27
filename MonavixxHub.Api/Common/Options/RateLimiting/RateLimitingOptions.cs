using System.ComponentModel.DataAnnotations;

namespace MonavixxHub.Api.Common.Options.RateLimiting;

public class RateLimitingOptions
{
    public const string Name = "RateLimiting";
    [Required] public FixedRateLimitingOptions Register { get; set; }
    [Required] public FixedRateLimitingOptions Login { get; set; }
}