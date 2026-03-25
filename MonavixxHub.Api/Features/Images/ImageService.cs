using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MonavixxHub.Api.Common.Options;
using MonavixxHub.Api.Features.Images.Exceptions;
using MonavixxHub.Api.Features.Images.Models;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.Images;
// TODO: Add compression
public class ImageService : IImageService
{
    private readonly AppDbContext _dbContext;
    private readonly StorageOptions _storageOptions;

    public ImageService(IOptions<StorageOptions> options, AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _storageOptions = options.Value;
    }

    public async ValueTask<byte[]> GetImageBytesAsync(Guid imageId)
    {
        var image = await _dbContext.Images.FindAsync(imageId);
        if (image is null) throw new ImageNotFoundException();
        return await File.ReadAllBytesAsync(Path.Combine(_storageOptions.ImageFolder, image.Path));
    }

    public async ValueTask<Image> GetImageAsync(Guid imageId)
    {
        return await _dbContext.Images.FindAsync(imageId) ?? throw new ImageNotFoundException();
    }

    public async ValueTask<Image> SaveImageAsync(IFormFile file, int initialReferenceCount = 1)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        return await SaveImageAsync(ms.ToArray(), file.ContentType);
    }

    public async ValueTask<Image> SaveImageAsync(byte[] image, string mimeType, int initialReferenceCount = 1)
    {
        Directory.CreateDirectory(_storageOptions.ImageFolder);
        byte[] hash = SHA256.HashData(image);
        var query = _dbContext.Images.Where(i => i.Hash == hash);
        await query.LoadAsync();
        foreach (var i in query)
        {
            if (image.SequenceEqual((await File.ReadAllBytesAsync(Path.Combine(_storageOptions.ImageFolder, i.Path)))
                    .AsEnumerable()))
            {
                i.ReferenceCount++;
                await _dbContext.SaveChangesAsync();
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
            ReferenceCount = initialReferenceCount,
        };
        _dbContext.Images.Add(img);
        await _dbContext.SaveChangesAsync();
        return img;
    }
    private string NewImageFilename(string mimeType)
    {
        MimeKit.MimeTypes.TryGetExtension(mimeType, out var extension);
        return Guid.NewGuid().ToString("N") + extension;
    }
    public async ValueTask DeleteImageAsync(Guid imageId)
    {
        Image? image = await _dbContext.Images.FindAsync(imageId);
        if(image is null) throw new ImageNotFoundException();
        _dbContext.Remove(image);
        DeleteImageFile(image);
        await _dbContext.SaveChangesAsync();
    }

    public async ValueTask DecrementRcAndDeleteIfUnusedAsync(Guid imageId)
    {
        Image? image = await _dbContext.Images.FindAsync(imageId);
        if(image is null) throw new ImageNotFoundException();
        if (--image.ReferenceCount <= 0)
        {
            DeleteImageFile(image);
            _dbContext.Remove(image);
        }
        await _dbContext.SaveChangesAsync();
    }

    private void DeleteImageFile(Image image)
    {
        File.Delete(Path.Combine(_storageOptions.ImageFolder, image.Path));
    }

    public async ValueTask IncrementRcAsync(Guid imageId)
    {
        int num = await _dbContext.Images.Where(i => i.Id == imageId).ExecuteUpdateAsync(u =>
        {
            u.SetProperty(x => x.ReferenceCount, x => x.ReferenceCount + 1);
        });
        if(num == 0)
            throw new ImageNotFoundException();
    }
}