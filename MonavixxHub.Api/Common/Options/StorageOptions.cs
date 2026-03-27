using System.ComponentModel.DataAnnotations;

namespace MonavixxHub.Api.Common.Options;

public class StorageOptions
{
    public const string Name = "Storage";
    [Required] public string ImageFolder { get; set; }
}