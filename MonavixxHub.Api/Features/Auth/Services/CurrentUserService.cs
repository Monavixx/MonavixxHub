using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.Auth.Services;


// todo: when I'll need to retrieve User entity, write this class and ProvideUserEntityAttribute and in middleware make decisions
public class CurrentUserService
{
    
    public CurrentUserService(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext)
    {
        
    }
}