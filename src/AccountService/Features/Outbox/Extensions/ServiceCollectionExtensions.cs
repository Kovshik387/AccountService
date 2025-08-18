using AccountService.Features.AccrueInterest.Jobs;
using AccountService.Features.Outbox.Jobs;

namespace AccountService.Features.Outbox.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJobs(this IServiceCollection services)
    {
        services.AddScoped<AccrueInterestJob>();
        services.AddScoped<OutboxPublisherJob>();
        
        return services;
    }
}