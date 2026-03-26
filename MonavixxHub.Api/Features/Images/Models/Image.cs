using System.ComponentModel.DataAnnotations;

namespace MonavixxHub.Api.Features.Images.Models;

public class Image
{
    public const int MimeTypeMaxLength = 255;
    [Required] public Guid Id { get; set; } = Guid.NewGuid();
    [Required] public string Path { get; set; }
    [Required] public string MimeType { get; set; }
    [Required] public byte[] Hash { get; set; }
    [Required] public int ReferenceCount { get; set; }
}