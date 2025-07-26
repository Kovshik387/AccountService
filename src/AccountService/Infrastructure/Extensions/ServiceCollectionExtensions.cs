using AccountService.Domain.Repositories;
using AccountService.Features.Interfaces;
using AccountService.Infrastructure.Repositories;
using AccountService.Infrastructure.Services;

namespace AccountService.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IAccountRepository, AccountRepositoryInMemory>();
        services.AddSingleton<ITransactionRepository, TransactionRepositoryInMemory>();
        
        return services;
    }

    public static IServiceCollection AddExternalServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IClientVerificationService, ClientVerificationService>();
        services.AddSingleton<ICurrencyVerificationService, CurrencyVerificationService>();
        
        return services;
    }
}