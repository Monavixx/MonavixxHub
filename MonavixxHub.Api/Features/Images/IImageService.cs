using MonavixxHub.Api.Features.Images.Models;

namespace MonavixxHub.Api.Features.Images;

public interface IImageService
{
    ValueTask<byte[]> GetImageBytesAsync(Guid imageId);
    ValueTask<Image> GetImageAsync(Guid imageId);
    ValueTask<Image> SaveImageAsync(IFormFile file, int initialReferenceCount = 1);
    ValueTask DeleteImageAsync(Guid imageId);
    ValueTask IncrementRcAsync(Guid imageId);
    ValueTask DecrementRcAndDeleteIfUnusedAsync(Guid imageId);
}