using AccountService.Domain.Repositories;
using AccountService.HealthCheck.Settings;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace AccountService.HealthCheck;

public class OutboxHealthCheck : IHealthCheck
{
    private readonly IOutboxRepository _outboxRepository;
    private readonly HealthOptions _healthOptions;

    public OutboxHealthCheck(IOutboxRepository outboxRepository, IOptions<HealthOptions> healthOptions)
    {
        _outboxRepository = outboxRepository;
        _healthOptions = healthOptions.Value;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var pending = await _outboxRepository.PendingCountAsync(cancellationToken);

            if (pending > _healthOptions.OutboxWarnThreshold)
            {
                return HealthCheckResult.Degraded(
                    description: $"Outbox backlog {pending} > {_healthOptions.OutboxWarnThreshold}",
                    data: new Dictionary<string, object?>
                    {
                        ["pending"] = pending,
                        ["threshold"] = _healthOptions.OutboxWarnThreshold
                    }!);
            }

            return HealthCheckResult.Healthy("Outbox within threshold",
                data: new Dictionary<string, object?>
                {
                    ["pending"] = pending,
                    ["threshold"] = _healthOptions.OutboxWarnThreshold
                }!);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Outbox check failed", ex);
        }
    }
}