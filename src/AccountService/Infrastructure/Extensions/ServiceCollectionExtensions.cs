using System.Reflection;
using AccountService.Domain.Repositories;
using AccountService.Features.Interfaces;
using AccountService.Infrastructure.Common;
using AccountService.Infrastructure.Repositories;
using AccountService.Infrastructure.Services;
using AccountService.Infrastructure.Settings;
using FluentMigrator.Runner;
using Microsoft.Extensions.Options;

namespace AccountService.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        var settingsSection = configuration.GetSection(nameof(DbOptions));
        services.Configure<DbOptions>(settingsSection);

        var settings = settingsSection.Get<DbOptions>();
        if (settings == null) throw new NullReferenceException("Db settings not found");

        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        services.AddSingleton<IPgRepository, PgRepository>();
        services.AddSingleton<IAccountRepository, AccountRepository>();
        services.AddSingleton<ITransactionRepository, TransactionRepository>();

        services.AddFluentMigrator(typeof(SqlMigration).Assembly);

        return services;
    }

    public static IServiceCollection AddExternalServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IClientVerificationService, ClientVerificationService>();
        services.AddSingleton<ICurrencyVerificationService, CurrencyVerificationService>();

        return services;
    }

    private static void AddFluentMigrator(this IServiceCollection services, Assembly assembly)
    {
        services
            .AddFluentMigratorCore()
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .ConfigureRunner(x => x
                .AddPostgres()
                .WithGlobalConnectionString(f =>
                    f.GetRequiredService<IOptions<DbOptions>>().Value.ConnectionString)
                .ScanIn(assembly).For.Migrations());
    }
}