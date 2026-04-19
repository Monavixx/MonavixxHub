using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Features.Auth.Services;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.Auth.Jobs;

public class SessionCleaningJob (ISessionService sessionService, ILogger<SessionCleaningJob> logger) : ISessionCleaningJob
{
    public async Task CleanAsync()
    {
        try
        {
            await sessionService.CleanExpiredSessionsAsync();
            logger.LogInformation("Successfully cleaned expired sessions");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to clean expired sessions");
            throw;
        }
    }
}