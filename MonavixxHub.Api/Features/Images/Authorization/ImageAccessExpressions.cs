using System.Linq.Expressions;
using MonavixxHub.Api.Features.Images.Models;

namespace MonavixxHub.Api.Features.Images.Authorization;

public static class ImageAccessExpressions
{
    public static Expression<Func<Image, bool>> Owns(UserIdType userId)
        => i => i.Flashcards.Any(f => f.OwnerId == userId);
}