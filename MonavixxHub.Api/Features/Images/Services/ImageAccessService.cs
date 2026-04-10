using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Features.Auth.Extensions;
using MonavixxHub.Api.Features.Flashcards.Authorization;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.Images.Services;

public class ImageAccessService(AppDbContext dbContext, ILogger<ImageAccessService> logger)
{
    public async ValueTask<bool> CanRead(Guid imageId, ClaimsPrincipal user)
    {
        logger.LogDebug("Checking user's read access to image [{imageId}]...", imageId);
        var userId = user.GetUserId();
        var hasAccess = await dbContext.Flashcards
            .Where(FlashcardAccessExpressions.CanRead(userId))
            .AnyAsync(flashcard => flashcard.ImageId == imageId);
        logger.LogDebug("Read access to image ({ImageId}) is {Access}",
            imageId, hasAccess ? "allowed" : "denied");
        return hasAccess;
    }
}