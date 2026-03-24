using MonavixxHub.Api.Common.Models;

namespace MonavixxHub.Api.Common;

public interface IImageService
{
    ValueTask<byte[]> GetImageBytesAsync(Guid imageId);
    ValueTask<Image?> GetImageAsync(Guid imageId);
    ValueTask<Image> SaveImageAsync(IFormFile file, int initialReferenceCount = 1);
    ValueTask DeleteImageAsync(Guid imageId);
    ValueTask IncrementRcAsync(Guid imageId);
    ValueTask DecrementRcAndDeleteIfUnusedAsync(Guid imageId);
}