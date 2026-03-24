using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MonavixxHub.Api.Common.Options;

namespace MonavixxHub.Api.Common;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ImageController : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async ValueTask<IActionResult> Get(Guid id, [FromServices] IImageService imageService,
        IOptions<StorageOptions> storageOptions)
    {
        var image = await imageService.GetImageAsync(id);
        if (image is null)
            return NotFound();
        return PhysicalFile(Path.GetFullPath(Path.Combine(storageOptions.Value.ImageFolder, image.Path)),
            image.MimeType);
    }
}