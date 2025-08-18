namespace AccountService.Domain.Events;

public static class EventFactory
{
    public static EventShell<TPayload> New<TPayload>(TPayload payload, string? correlationId = null, string? causationId = null)
        => new EventShell<TPayload>
        {
            EventId = Guid.NewGuid(),
            OccurredAt = DateTimeOffset.UtcNow,
            Meta = new Meta
            {
                Version = "v1",
                Source = "account-service",
                CorrelationId = correlationId ?? string.Empty,
                CausationId = causationId ?? string.Empty
            },
            Payload = payload
        };
}