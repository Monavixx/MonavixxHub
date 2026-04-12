using MonavixxHub.Api.Features.Auth.Exceptions;
using MonavixxHub.Api.Features.Auth.Extensions;
using MonavixxHub.Api.Features.Auth.Models;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.Auth.Services;


public class CurrentUserService(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext) : ICurrentUserService
{
    private User? _user;
    public async ValueTask<User> GetUserAsync() 
        => _user ??= await dbContext.Users.FindAsync(httpContextAccessor.HttpContext!.User.GetUserId())
        ?? throw new UserDoesNotExistException();
}