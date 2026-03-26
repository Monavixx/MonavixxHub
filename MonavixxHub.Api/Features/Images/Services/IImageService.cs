using MonavixxHub.Api.Features.Images.Exceptions;
using MonavixxHub.Api.Features.Images.Models;

namespace MonavixxHub.Api.Features.Images.Services;

/// <summary>
/// Provides methods to store, retrieve and manage images.
/// </summary>
public interface IImageService
{
    /// <summary>
    /// Returns the raw bytes of the specified image.
    /// </summary>
    /// <param name="imageId">ID of the image to retrieve.</param>
    /// <returns>Raw image bytes.</returns>
    /// <exception cref="ImageNotFoundException">
    /// Thrown if the image doesn't exist.
    /// </exception>
    ValueTask<byte[]> GetImageBytesAsync(Guid imageId);
    /// <summary>
    /// Returns the image entity with the specified ID.
    /// </summary>
    /// <param name="imageId">ID of the image to retrieve.</param>
    /// <returns>The image entity</returns>
    ValueTask<Image> GetImageAsync(Guid imageId);
    /// <summary>
    /// Saves the image from the specified form <paramref name="file"/> and returns the stored entity.
    /// </summary>
    /// <param name="file">The uploaded image file to store.</param>
    /// <param name="addReferenceCount">Initial or additional reference count. Default to 1.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="addReferenceCount"/> is less than 1.
    /// </exception>
    /// <returns>The saved or existing image entity.</returns>
    ValueTask<Image> SaveImageAsync(IFormFile file, int addReferenceCount = 1);
    /// <summary>
    /// Increments reference count of the specified image.
    /// </summary>
    /// <param name="imageId">ID of the image.</param>
    /// <exception cref="ImageNotFoundException">
    /// Thrown if the image doesn't exist.
    /// </exception>
    ValueTask IncrementRcAsync(Guid imageId);
    /// <summary>
    /// Decrements reference count of the specified image.
    /// Then, if the reference count is less than 1, deletes the image.
    /// </summary>
    /// <param name="imageId">ID of the image.</param>
    /// <exception cref="ImageNotFoundException">
    /// Thrown if the image doesn't exist.
    /// </exception>
    ValueTask DecrementRcAndDeleteIfUnusedAsync(Guid imageId);
}