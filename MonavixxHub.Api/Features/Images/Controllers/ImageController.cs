using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MonavixxHub.Api.Common.Options;
using MonavixxHub.Api.Features.Images.Authorization;
using MonavixxHub.Api.Features.Images.Services;

namespace MonavixxHub.Api.Features.Images.Controllers;

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
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async ValueTask<IActionResult> Get(Guid id, [FromServices] IImageService imageService,
        IOptions<StorageOptions> storageOptions, [FromServices] IAuthorizationService authorizationService)
    {
        var image = await imageService.GetImageAsync(id);
        if (!(await authorizationService.AuthorizeAsync(User, image, Requirements.ReadAccess)).Succeeded)
            return Forbid();
        return PhysicalFile(Path.GetFullPath(Path.Combine(storageOptions.Value.ImageFolder, image.Path)),
            image.MimeType);
    }
}