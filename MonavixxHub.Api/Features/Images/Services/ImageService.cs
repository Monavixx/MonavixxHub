using System.IO.Compression;
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

    public async ValueTask<Stream> GetImageStreamAsync(Image image)
    {
        return new DeflateStream(File.OpenRead(Path.Combine(_storageOptions.ImageFolder, image.Path)),
            CompressionMode.Decompress);
    }

    public async ValueTask<Image> GetImageAsync(Guid imageId)
    {
        return await dbContext.Images.FindAsync(imageId) ?? throw new ImageNotFoundException();
    }

    // public async ValueTask<Image> SaveImageAsync(IFormFile file, int addReferenceCount = 1)
    // {
    //     if(addReferenceCount < 1) throw new ArgumentOutOfRangeException(nameof(addReferenceCount));
    //     using var ms = new MemoryStream();
    //     await file.CopyToAsync(ms);
    //     return await SaveImageAsync(ms.ToArray(), file.ContentType, addReferenceCount);
    // }

    public async ValueTask<Image> SaveImageAsync(Stream imageStream, string mimeType, int addReferenceCount = 1)
    {
        if(addReferenceCount < 1) throw new ArgumentOutOfRangeException(nameof(addReferenceCount));
        
        using var ms = new MemoryStream();
        await imageStream.CopyToAsync(ms);
        byte[] image = ms.ToArray();
        
        byte[] hash = SHA256.HashData(image);
        var imageEntity = await GetOrNullAsync(image, hash);
        if (imageEntity is not null)
        {
            imageEntity.ReferenceCount += addReferenceCount;
            await dbContext.SaveChangesAsync();
            return imageEntity;
        }
        return await SaveNewImageAsync(image, hash, mimeType, addReferenceCount);
    }

    private async ValueTask<Image> SaveNewImageAsync(byte[] image, byte[] hash, string mimeType, int addReferenceCount)
    {
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
    private string NewImageFilename(/*string mimeType*/)
    {
        // MimeKit.MimeTypes.TryGetExtension(mimeType, out var extension);
        return Guid.NewGuid().ToString("N")/* + extension + ".deflate"*/;
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
        if (num == 0)
            throw new ImageNotFoundException();
    }
}