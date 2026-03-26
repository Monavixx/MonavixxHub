using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MonavixxHub.Api.Common.Options;
using MonavixxHub.Api.Features.Images.Exceptions;
using MonavixxHub.Api.Features.Images.Models;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.Images.Services;
// TODO: Add compression

/// <summary>
/// Provides memory-efficient methods to create, retrieve and delete images.
/// Stores images as files on the local filesystem.
/// Deduplicates images using SHA-256 hashing with byte-level verification.
/// </summary>
public class ImageService(IOptions<StorageOptions> options, AppDbContext dbContext) : IImageService
{
    private readonly StorageOptions _storageOptions = options.Value;

    public async ValueTask<byte[]> GetImageBytesAsync(Guid imageId)
    {
        var image = await dbContext.Images.FindAsync(imageId);
        if (image is null) throw new ImageNotFoundException();
        return await File.ReadAllBytesAsync(Path.Combine(_storageOptions.ImageFolder, image.Path));
    }

    public async ValueTask<Image> GetImageAsync(Guid imageId)
    {
        return await dbContext.Images.FindAsync(imageId) ?? throw new ImageNotFoundException();
    }

    public async ValueTask<Image> SaveImageAsync(IFormFile file, int addReferenceCount = 1)
    {
        if(addReferenceCount < 1) throw new ArgumentOutOfRangeException(nameof(addReferenceCount));
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        return await SaveImageAsync(ms.ToArray(), file.ContentType, addReferenceCount);
    }

    public async ValueTask<Image> SaveImageAsync(byte[] image, string mimeType, int addReferenceCount = 1)
    {
        if(addReferenceCount < 1) throw new ArgumentOutOfRangeException(nameof(addReferenceCount));
        
        Directory.CreateDirectory(_storageOptions.ImageFolder);
        byte[] hash = SHA256.HashData(image);
        var query = dbContext.Images.Where(i => i.Hash == hash);
        await query.LoadAsync();
        foreach (var i in query)
        {
            if (image.SequenceEqual(await File.ReadAllBytesAsync(Path.Combine(_storageOptions.ImageFolder, i.Path))))
            {
                i.ReferenceCount += addReferenceCount;
                await dbContext.SaveChangesAsync();
                return i;
            }
        }

        string path = NewImageFilename(mimeType);
        await File.WriteAllBytesAsync(Path.Combine(_storageOptions.ImageFolder, path), image);

        Image img = new()
        {
            Hash = hash,
            Path = path,
            MimeType = mimeType,
            ReferenceCount = addReferenceCount,
        };
        dbContext.Images.Add(img);
        await dbContext.SaveChangesAsync();
        return img;
    }
    private string NewImageFilename(string mimeType)
    {
        MimeKit.MimeTypes.TryGetExtension(mimeType, out var extension);
        return Guid.NewGuid().ToString("N") + extension;
    }
    public async ValueTask DeleteImageAsync(Guid imageId)
    {
        Image? image = await dbContext.Images.FindAsync(imageId);
        if(image is null) throw new ImageNotFoundException();
        dbContext.Remove(image);
        DeleteImageFile(image);
        await dbContext.SaveChangesAsync();
    }

    public async ValueTask DecrementRcAndDeleteIfUnusedAsync(Guid imageId)
    {
        Image? image = await dbContext.Images.FindAsync(imageId);
        if(image is null) throw new ImageNotFoundException();
        if (--image.ReferenceCount <= 0)
        {
            DeleteImageFile(image);
            dbContext.Remove(image);
        }
        await dbContext.SaveChangesAsync();
    }

    private void DeleteImageFile(Image image)
    {
        File.Delete(Path.Combine(_storageOptions.ImageFolder, image.Path));
    }

    public async ValueTask IncrementRcAsync(Guid imageId)
    {
        int num = await dbContext.Images.Where(i => i.Id == imageId).ExecuteUpdateAsync(u =>
        {
            u.SetProperty(x => x.ReferenceCount, x => x.ReferenceCount + 1);
        });
        if(num == 0)
            throw new ImageNotFoundException();
    }
}