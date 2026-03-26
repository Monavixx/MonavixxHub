using System.ComponentModel.DataAnnotations;

namespace MonavixxHub.Api.Features.Images.Models;

public class Image
{
    public const int MimeTypeMaxLength = 255;
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Path { get; set; }
    public string MimeType { get; set; }
    public byte[] Hash { get; set; }
    public int ReferenceCount { get; set; }
}