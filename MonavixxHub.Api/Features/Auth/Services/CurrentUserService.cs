using MonavixxHub.Api.Features.Auth.Exceptions;
using MonavixxHub.Api.Features.Auth.Extensions;
using MonavixxHub.Api.Features.Auth.Models;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.Auth.Services;

/// <summary>
/// Provides access to the currently authenticated user from the HTTP context.
/// </summary>
public class CurrentUserService(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext) : ICurrentUserService
{
    private User? _user;

    /// <summary>
    /// Gets the current user from the HTTP context.
    /// </summary>
    /// <returns>The current user entity.</returns>
    /// <exception cref="UserDoesNotExistException">Thrown if the user is not found in the database.</exception>
    public async ValueTask<User> GetUserAsync()
        => _user ??= await dbContext.Users.FindAsync(httpContextAccessor.HttpContext!.User.GetUserId())
        ?? throw new UserDoesNotExistException();
}