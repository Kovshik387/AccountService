using System.Text.Json;
using AccountService.Domain.Repositories;
using AccountService.Features.Interfaces;
using Hangfire;
using Polly;

namespace AccountService.Features.Outbox.Jobs;

public sealed class OutboxPublisherJob
{
    private readonly IOutboxRepository _outboxRepository;
    private readonly IPgRepository _pgRepository;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<OutboxPublisherJob> _logger;
    private readonly IAsyncPolicy _policy;

    public OutboxPublisherJob(IOutboxRepository outboxRepository, IMessageBus messageBus,
        ILogger<OutboxPublisherJob> logger, IPgRepository pgRepository)
    {
        _outboxRepository = outboxRepository;
        _messageBus = messageBus;
        _logger = logger;
        _pgRepository = pgRepository;
        _policy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt =>
                {
                    if (attempt == 1) return TimeSpan.Zero;
                    return TimeSpan.FromSeconds(Math.Pow(2, attempt - 1));
                },
                onRetryAsync: async (ex, delay, attempt, context) =>
                {
                    _logger.LogWarning(ex,
                        "publish retry {Attempt}, delay={Delay}ms, id={Id}",
                        attempt, (int)delay.TotalMilliseconds, context.GetValueOrDefault("msgId"));
                    await Task.CompletedTask;
                });
    }

    [Queue("outbox")]
    [AutomaticRetry(Attempts = 0)]
    public async Task RunAsync(IJobCancellationToken token, int batchSize = 200, int leaseSeconds = 60)
    {
        _logger.LogInformation("Starting outbox publisher");
        var shutdownToken = CancellationToken.None;

        var batch = await _outboxRepository.GetEventsBatchAsync(batchSize, shutdownToken);
        if (batch.Count == 0)
        {
            _logger.LogDebug("outbox: nothing to dispatch");
            return;
        }

        var ids = new List<Guid>(batch.Count);

        foreach (var item in batch)
        {
            try
            {
                using var doc = JsonDocument.Parse(item.Data);
                var payload = doc.RootElement.Clone();

                var context = new Context { ["msgId"] = item.Id };

                await _policy.ExecuteAsync(
                    async (_, cancellationToken) =>
                        await _messageBus.PushAsync(item.RoutingKey, payload, cancellationToken),
                    context,
                    shutdownToken);

                ids.Add(item.Id);
            }
            catch (Exception ex)
            {
                var nextAttempt = item.AttemptCount + 1;
                var baseSeconds = Math.Pow(2, Math.Max(0, nextAttempt - 1));
                var jitter = Random.Shared.NextDouble() * 0.3;
                var delay = TimeSpan.FromSeconds(baseSeconds + jitter);

                var cap = TimeSpan.FromMinutes(5);
                if (delay > cap) delay = cap;

                await _outboxRepository.ScheduleAsync(item.Id, delay, ex.ToString(), shutdownToken);

                _logger.LogWarning(ex,
                    "outbox: publish failed (exhausted retries) id={Id}, attempts={Attempts}, next in {Delay}s",
                    item.Id, item.AttemptCount, (int)delay.TotalSeconds);
            }
        }

        if (ids.Count > 0)
        {
            await using var conn = await _pgRepository.OpenConnectionAsync(shutdownToken);
            await using var tx = await conn.BeginTransactionAsync(shutdownToken);
            await _outboxRepository.DeleteEventAsync(conn, tx, ids.ToArray(), shutdownToken);
            await tx.CommitAsync(shutdownToken);
        }

        _logger.LogInformation("outbox: acquired={acq}, published={pub}, failed={fail}",
            batch.Count, ids.Count, batch.Count - ids.Count);
    }
}