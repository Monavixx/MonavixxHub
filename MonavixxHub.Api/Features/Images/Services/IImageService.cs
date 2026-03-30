using MonavixxHub.Api.Features.Images.Exceptions;
using MonavixxHub.Api.Features.Images.Models;

namespace MonavixxHub.Api.Features.Images.Services;

/// <summary>
/// Provides methods to store, retrieve and manage images.
/// </summary>
public interface IImageService
{
    /// <summary>
    /// Returns the stream of raw bytes of the specified image.
    /// </summary>
    /// <param name="image">Image to retrieve.</param>
    /// <returns>Raw image bytes.</returns>
    Stream GetImageStream(Image image);
    /// <summary>
    /// Returns the image entity with the specified ID.
    /// </summary>
    /// <param name="imageId">ID of the image to retrieve.</param>
    /// <returns>The image entity</returns>
    ValueTask<Image> GetImageAsync(Guid imageId);

    /// <summary>
    /// Saves the image from the specified <paramref name="stream"/> and returns the stored entity.
    /// </summary>
    /// <param name="stream">The uploaded image to store.</param>
    /// <param name="mimeType">MIME type of the uploaded image.</param>
    /// <param name="addReferenceCount">Initial or additional reference count. Default to 1.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="addReferenceCount"/> is less than 1.
    /// </exception>
    /// <returns>The saved or existing image entity.</returns>
    Task<Image> SaveImageAsync(Stream stream, string mimeType, int addReferenceCount = 1);
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
    Task DecrementRcAndDeleteIfUnusedAsync(Guid imageId);
    Task DecrementRcAndDeleteIfUnusedAsync(ICollection<Guid> imageIds);
}