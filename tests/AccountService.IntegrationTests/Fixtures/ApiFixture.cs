using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;
using AccountService.Infrastructure.Extensions;
using AccountService.IntegrationTests.Fakers;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.RabbitMq;

namespace AccountService.IntegrationTests.Fixtures;

public class ApiFixture<TProgram>
    : WebApplicationFactory<TProgram>, IAsyncLifetime where TProgram : class
{
    public HttpClient Client { get; private set; } = null!;
    private string ConnectionString => PgContainer.GetConnectionString();

    public readonly PostgreSqlContainer PgContainer;
    public readonly RabbitMqContainer RabbitMqContainer;

    public ApiFixture()
    {
        PgContainer = new PostgreSqlBuilder()
            .WithDatabase("integrations_db")
            .WithUsername("postgres")
            .WithPassword("test_postgres")
            .WithImage("postgres:latest")
            .Build();
        
        RabbitMqContainer = new RabbitMqBuilder()
            .WithImage("rabbitmq:management")
            .WithUsername("user")
            .WithPassword("user")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5672)) 
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
            var rmqPort = RabbitMqContainer.GetMappedPublicPort(5672);
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["DbOptions:ConnectionString"] = ConnectionString,
                ["RabbitMqOptions:Host"]       = "localhost",
                ["RabbitMqOptions:Port"]       = rmqPort.ToString(),
                ["RabbitMqOptions:UserName"]   = "user",
                ["RabbitMqOptions:Password"]   = "user",
                ["RabbitMqOptions:VHost"]= "/"
            });
        });
        var host = base.CreateHost(builder);

        // ClearDatabase(host);
        host.MigrateUp();

        return host;
    }

    public async Task InitializeAsync()
    {
        await PgContainer.StartAsync();
        await RabbitMqContainer.StartAsync();
        await Task.Delay(5000);
        Client = CreateClient();
    }

    public new async Task DisposeAsync()
    {
        Client.Dispose();
        await RabbitMqContainer.DisposeAsync();
        await PgContainer.DisposeAsync();
    }

    // private static void ClearDatabase(IHost host)
    // {
    //     using var scope = host.Services.CreateScope();
    //     var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    //     runner.MigrateDown(0);
    // }
}
