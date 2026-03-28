using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Features.Auth.Extensions;
using MonavixxHub.Api.Features.Flashcards.Authorization;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.Images.Services;

public class ImageAccessService (AppDbContext dbContext)
{
    public async ValueTask<bool> CanRead(Guid imageId, ClaimsPrincipal user)
    {
        var userId = user.GetUserId();
        return await dbContext.Flashcards
            .Where(FlashcardAccessExpressions.CanRead(userId))
            .AnyAsync(flashcard => flashcard.ImageId == imageId);
    }
}