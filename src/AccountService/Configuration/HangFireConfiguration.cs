using AccountService.Configuration.Settings;
using AccountService.Features.AccrueInterest.Jobs;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;

namespace AccountService.Configuration;

public static class HangFireConfiguration
{
    public static IServiceCollection AddHangFire(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfireServer();
        services.AddHangfire(x => { x.UseMemoryStorage(); });
        services.Configure<IdentityOptions>(configuration.GetSection(nameof(HangFireOptions)));
        
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
            "daily-accrue-interest-job",
            job => job.InvokeAsync(),
            Cron.Daily
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