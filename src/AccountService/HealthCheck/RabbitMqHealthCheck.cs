using AccountService.HealthCheck.Settings;
using AccountService.Infrastructure.Settings;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace AccountService.HealthCheck;

public class RabbitMqHealthCheck : IHealthCheck
{
    private readonly RabbitMqOptions _rabbitMqOptions;
    private readonly HealthOptions _healthOptions;


    public RabbitMqHealthCheck(IOptions<RabbitMqOptions> rabbitMqOptions, IOptions<HealthOptions> healthOptions)
    {
        _rabbitMqOptions = rabbitMqOptions.Value;
        _healthOptions = healthOptions.Value;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _rabbitMqOptions.Host,
                Port = _rabbitMqOptions.Port,
                UserName = _rabbitMqOptions.UserName,
                Password = _rabbitMqOptions.Password,
                VirtualHost = _rabbitMqOptions.VHost,
                AutomaticRecoveryEnabled = false
            };

            using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cancellationTokenSource.CancelAfter(_healthOptions.RabbitMqTimeoutMs);

            await using var connection = await factory.CreateConnectionAsync(cancellationTokenSource.Token);
            await using var channel =
                await connection.CreateChannelAsync(cancellationToken: cancellationTokenSource.Token);

            return HealthCheckResult.Healthy("RabbitMQ reachable");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("RabbitMQ unreachable", ex);
        }
    }
}