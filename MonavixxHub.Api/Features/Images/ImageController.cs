using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MonavixxHub.Api.Common.Options;

namespace MonavixxHub.Api.Features.Images;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ImageController : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status206PartialContent)]
    [ProducesResponseType(StatusCodes.Status416RangeNotSatisfiable)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> Get(Guid id, [FromServices] IImageService imageService,
        IOptions<StorageOptions> storageOptions)
    {
        var image = await imageService.GetImageAsync(id);
        return PhysicalFile(Path.GetFullPath(Path.Combine(storageOptions.Value.ImageFolder, image.Path)),
            image.MimeType);
    }
}