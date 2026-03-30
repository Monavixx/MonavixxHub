using System.IO.Compression;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MonavixxHub.Api.Common.Options;
using MonavixxHub.Api.Features.Images.Authorization;
using MonavixxHub.Api.Features.Images.Exceptions;
using MonavixxHub.Api.Features.Images.Models;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.Images.Services;

/// <summary>
/// Provides memory-efficient methods to create, retrieve and delete images.
/// Stores images as files on the local filesystem.
/// Deduplicates images using SHA-256 hashing with byte-level verification.
/// </summary>
public class ImageService(IOptions<StorageOptions> options, AppDbContext dbContext,
    ILogger<ImageService> logger) : IImageService
{
    private readonly StorageOptions _storageOptions = options.Value;

    public Stream GetImageStream(Image image)
    {
        logger.LogTrace("Getting image stream [{ImageId}]", image.Id);
        return new DeflateStream(File.OpenRead(Path.Combine(_storageOptions.ImageFolder, image.Path)),
            CompressionMode.Decompress);
    }

    public async ValueTask<Image> GetImageAsync(Guid imageId)
    {
        logger.LogDebug("Getting image [{ImageId}]", imageId);
        return await dbContext.Images.FindAsync(imageId) ?? throw new ImageNotFoundException();
    }

    public async Task<Image> SaveImageAsync(Stream imageStream, string mimeType, int addReferenceCount = 1)
    {
        if (addReferenceCount < 1) throw new ArgumentOutOfRangeException(nameof(addReferenceCount));
        logger.LogDebug("Saving image from stream...");
        using var ms = new MemoryStream();
        await imageStream.CopyToAsync(ms);
        byte[] image = ms.ToArray();

        byte[] hash = SHA256.HashData(image);
        var imageEntity = await GetOrNullAsync(image, hash);
        if (imageEntity is not null)
        {
            logger.LogDebug(
                "The image [{ImageId}] is already present in the database. Updating reference count...",
                imageEntity.Id);
            imageEntity.ReferenceCount += addReferenceCount;
            await dbContext.SaveChangesAsync();
            logger.LogInformation("The image's [{ImageId}] reference count has been updated successfully",
                imageEntity.Id);
            return imageEntity;
        }
        
        return await SaveNewImageAsync(image, hash, mimeType, addReferenceCount);
    }

    private async ValueTask<Image> SaveNewImageAsync(byte[] image, byte[] hash, string mimeType, int addReferenceCount)
    {
        logger.LogDebug("Saving new image...");
        Directory.CreateDirectory(_storageOptions.ImageFolder);
        string path = Path.Combine(_storageOptions.ImageFolder, NewImageFilename());
        var fs = new FileStream(path, FileMode.CreateNew);
        await using (var ds = new DeflateStream(fs, CompressionMode.Compress))
        {
            await ds.WriteAsync(image);
        }

        Image imageEntity = new()
        {
            Hash = hash,
            Path = path,
            MimeType = mimeType,
            ReferenceCount = addReferenceCount,
        };
        dbContext.Images.Add(imageEntity);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("New image [{ImageId}] has been saved successfully", imageEntity.Id);
        return imageEntity;
    }
    private async ValueTask<Image?> GetOrNullAsync(byte[] image, byte[] hash)
    {
        var query = dbContext.Images.Where(i => i.Hash == hash);
        await query.LoadAsync();
        foreach (var i in query)
        {
            var fs = File.OpenRead(Path.Combine(_storageOptions.ImageFolder, i.Path));
            await using var ds = new DeflateStream(fs, CompressionMode.Decompress);
            using var ms = new MemoryStream();
            await ds.CopyToAsync(ms);
            if (image.SequenceEqual(ms.GetBuffer().AsSpan(0, (int)ms.Length)))
                return i;
        }

        return null;
    }
    private string NewImageFilename()
    {
        return Guid.NewGuid().ToString("N");
    }
    public async ValueTask DeleteImageAsync(Guid imageId)
    {
        logger.LogDebug("Deleting image [{ImageId}]...", imageId);
        Image? image = await dbContext.Images.FindAsync(imageId);
        if(image is null) throw new ImageNotFoundException();
        dbContext.Remove(image);
        DeleteImageFile(image);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Image [{ImageId}] deleted successfully", imageId);
    }

    public async Task DecrementRcAndDeleteIfUnusedAsync(Guid imageId)
    {
        logger.LogDebug("Decrementing image's [{ImageId}] reference count...", imageId);
        Image? image = await dbContext.Images.FindAsync(imageId);
        if(image is null) throw new ImageNotFoundException();
        if (--image.ReferenceCount <= 0)
        {
            logger.LogDebug("Deleting image [{ImageId}] as its reference count is less than 1...", imageId);
            DeleteImageFile(image);
            dbContext.Remove(image);
        }
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Changes have been applied to the image [{ImageId}] successfully", imageId);
    }

    public async Task DecrementRcAndDeleteIfUnusedAsync(ICollection<Guid> imageIds)
    {
        logger.LogDebug("Decrementing images' reference count...");
        
        foreach (var image in dbContext.Images.Where(i => imageIds.Contains(i.Id)))
        {
            image.ReferenceCount -= imageIds.Count(g => g == image.Id);
            if (image.ReferenceCount <= 0)
            {
                logger.LogDebug("Deleting image [{ImageId}] as its reference count is less than 1...", image.Id);
                DeleteImageFile(image);
                dbContext.Remove(image);
            }
        }
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Changes have been applied to the images successfully");
    }

    public IQueryable<Image> GetUsersImages(UserIdType userId)
    {
        return dbContext.Images.Where(ImageAccessExpressions.Owns(userId));
    }

    private void DeleteImageFile(Image image)
    {
        File.Delete(Path.Combine(_storageOptions.ImageFolder, image.Path));
        logger.LogDebug("Image file [ImageId: {ImageId}] deleted successfully", image.Id);
    }

    public async ValueTask IncrementRcAsync(Guid imageId)
    {
        int num = await dbContext.Images.Where(i => i.Id == imageId).ExecuteUpdateAsync(u =>
        {
            u.SetProperty(x => x.ReferenceCount, x => x.ReferenceCount + 1);
        });
        if (num == 0)
            throw new ImageNotFoundException();
    }
}