using System.Text.Json;
using AccountService.Converters;
using AccountService.Domain.Events;
using AccountService.Domain.Events.Consume;
using AccountService.Domain.Repositories;
using AccountService.Features.Interfaces;
using Hangfire;

namespace AccountService.Features.Outbox.Jobs;

public class OutboxReceiverJob
{
    private readonly IInBoxRepository _repository;
    private readonly IAccountRepository _accountRepository;
    private readonly IPgRepository _pgRepository;
    private readonly IInBoxDeadLettersRepository _deadLettersRepository;

    private readonly IMessageBus _messageBus;

    private readonly ILogger<OutboxReceiverJob> _logger;

    private readonly RouteOptions _routeOptions;

    private const string SupportedVersion = "v1";

    public OutboxReceiverJob(IInBoxRepository repository, ILogger<OutboxReceiverJob> logger, IMessageBus messageBus,
        RouteOptions routeOptions, IAccountRepository accountRepository, IPgRepository pgRepository,
        IInBoxDeadLettersRepository deadLettersRepository)
    {
        _repository = repository;
        _logger = logger;
        _messageBus = messageBus;
        _routeOptions = routeOptions;
        _accountRepository = accountRepository;
        _pgRepository = pgRepository;
        _deadLettersRepository = deadLettersRepository;
    }

    [Queue("inbox")]
    [AutomaticRetry(Attempts = 0)]
    public async Task RunAsync(IJobCancellationToken token)
    {
        var shutdownToken = token.ShutdownToken;

        await _messageBus.Subscribe<EventShell<ClientBlocked>>(_routeOptions.ClientBlockEventRoute,
            async handle =>
            {
                var success = await ValidateOrQuarantine(handle, "ClientBlockedHandler",
                    shutdownToken);
                if (!success) return;
                
                await using var connection = await _pgRepository.OpenConnectionAsync(shutdownToken);
                await using var transaction = await connection.BeginTransactionAsync(shutdownToken);

                if (!await _repository.MarkProcessingAsync(handle.EventId, nameof(ClientBlocked), connection,
                        transaction, shutdownToken))
                {
                    return;
                }

                await _accountRepository.BlockAccountAsync(handle.Payload.ClientId, connection, transaction,
                    shutdownToken);
                await transaction.CommitAsync(shutdownToken);
            },
            shutdownToken);

        await _messageBus.Subscribe<EventShell<ClientUnBlocked>>(_routeOptions.ClientUnblockEventRoute,
            async handle =>
            {
                var success = await ValidateOrQuarantine(handle, "ClientUnBlockedHandler",
                    shutdownToken);
                if (!success) return;

                await using var connection = await _pgRepository.OpenConnectionAsync(shutdownToken);
                await using var transaction = await connection.BeginTransactionAsync(shutdownToken);

                if (!await _repository.MarkProcessingAsync(handle.Payload.ClientId, nameof(ClientUnBlocked), connection,
                        transaction, shutdownToken))
                {
                    return;
                }

                await _accountRepository.UnblockAccountAsync(handle.Payload.ClientId, connection, transaction,
                    shutdownToken);
                await transaction.CommitAsync(shutdownToken);
            },
            shutdownToken);
    }

    private async Task<bool> ValidateOrQuarantine<TPayload>(
        EventShell<TPayload> shell,
        string handlerName,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(shell.Meta.Version))
            return await Quarantine(shell, handlerName, "Missing Meta/Version", cancellationToken);

        if (!shell.Meta.Version.Equals(SupportedVersion, StringComparison.OrdinalIgnoreCase))
            return await Quarantine(shell, handlerName,
                $"Unsupported version '{shell.Meta.Version}' (supported '{SupportedVersion}')", cancellationToken);

        return true;
    }

    private async Task<bool> Quarantine<TPayload>(
        EventShell<TPayload> shell,
        string handlerName,
        string error,
        CancellationToken cancellationToken)
    {
        var payloadJson = JsonSerializer.Serialize(shell, JsonOption.Options);

        await _deadLettersRepository.AddAsync(shell.EventId, handlerName, payloadJson, error, cancellationToken);
        _logger.LogWarning("Inbox quarantine: {Error}; handler={Handler}; eventId={EventId}",
            error, handlerName, shell.EventId);

        return false;
    }
}