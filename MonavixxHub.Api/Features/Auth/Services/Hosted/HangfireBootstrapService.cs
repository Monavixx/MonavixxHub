using Hangfire;
using MonavixxHub.Api.Features.Auth.Jobs;

namespace MonavixxHub.Api.Features.Auth.Services.Hosted;

public class HangfireBootstrapService (IRecurringJobManagerV2 recurringJobManager): IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        recurringJobManager.AddOrUpdate<ISessionCleaningJob>("session-cleaning",
            s => s.CleanAsync(), Cron.Minutely);
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}