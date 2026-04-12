using System.Security.Claims;

namespace MonavixxHub.Api.Features.Images.Services;

/// <summary>
/// Provides methods to check whether a specific user has access to an image.
/// </summary>
public interface IImageAccessService
{
    /// <summary>
    /// Determines whether the specified user has read access to the given image.
    /// </summary>
    /// <param name="imageId">ID of the image to check access for.</param>
    /// <param name="user">User whose access is being checked.</param>
    /// <returns><c>true</c> if the user can read the image; otherwise, <c>false</c>.</returns>
    ValueTask<bool> CanRead(Guid imageId, ClaimsPrincipal user);
}

