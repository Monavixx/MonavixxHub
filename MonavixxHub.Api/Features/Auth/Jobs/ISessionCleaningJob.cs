namespace MonavixxHub.Api.Features.Auth.Jobs;

public interface ISessionCleaningJob
{
    Task CleanAsync();
}