using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;
using AccountService.Infrastructure.Extensions;
using AccountService.IntegrationTests.Fakers;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace AccountService.IntegrationTests.Fixtures;

public class ApiFixture<TProgram>
    : WebApplicationFactory<TProgram>, IAsyncLifetime where TProgram : class
{
    public HttpClient Client { get; private set; } = null!;
    private string ConnectionString => _pgContainer.GetConnectionString();

    private readonly PostgreSqlContainer _pgContainer;

    public ApiFixture()
    {
        _pgContainer = new PostgreSqlBuilder()
            .WithDatabase("integrations_db")
            .WithUsername("postgres")
            .WithPassword("test_postgres")
            .WithImage("postgres:latest")
            .Build();
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = AuthFaker.Scheme;
                    options.DefaultChallengeScheme = AuthFaker.Scheme;
                    options.DefaultScheme = AuthFaker.Scheme;
                })
                .AddScheme<AuthenticationSchemeOptions, AuthFaker>(AuthFaker.Scheme, _ => { });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy =
                    new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder(AuthFaker.Scheme)
                        .RequireAuthenticatedUser()
                        .Build();
                options.FallbackPolicy = options.DefaultPolicy;
            });
        });


        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["DbOptions:ConnectionString"] = ConnectionString
            });
        });
        var host = base.CreateHost(builder);

        // ClearDatabase(host);
        host.MigrateUp();

        return host;
    }

    public async Task InitializeAsync()
    {
        await _pgContainer.StartAsync();
        Client = CreateClient();
    }

    public new async Task DisposeAsync()
    {
        Client.Dispose();
        await _pgContainer.DisposeAsync();
    }

    // private static void ClearDatabase(IHost host)
    // {
    //     using var scope = host.Services.CreateScope();
    //     var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    //     runner.MigrateDown(0);
    // }
}
