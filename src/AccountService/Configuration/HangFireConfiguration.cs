using AccountService.Configuration.Settings;
using AccountService.Features.AccrueInterest.Jobs;
using AccountService.Features.Outbox.Jobs;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;

namespace AccountService.Configuration;

public static class HangFireConfiguration
{
    public static IServiceCollection AddHangFire(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfireServer(x =>
        {
            x.Queues = ["default", "outbox","inbox"];
        });
        services.AddHangfire(x => { x.UseMemoryStorage(); });
        services.Configure<HangFireOptions>(configuration.GetSection(nameof(HangFireOptions)));
        
        return services;
    }
    
    public static void UseHangFireConfiguration(this IApplicationBuilder app)
    {
        app.UseHangfireDashboard(options: new DashboardOptions
        {
            Authorization =
            [
                new AllowAllDashboardAuthorizationFilter()
            ]
        });
        RecurringJob.AddOrUpdate<AccrueInterestJob>(
            "minutely-accrue-interest-job",
            job => job.InvokeAsync(),
            Cron.Daily
            );
        
        RecurringJob.AddOrUpdate<OutboxPublisherJob>(
            "minutely-outbox-publisher-job",
            job => job.RunAsync(JobCancellationToken.Null, 10, 60),
            Cron.Minutely
            );
        RecurringJob.AddOrUpdate<AntifraudConsumer>(
            "inbox-publisher-job",
            job => job.RunAsync(JobCancellationToken.Null),
            Cron.Minutely
            );
    }
    
    private class AllowAllDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}