namespace AccountService.Domain.Events;

public record EventShell<TPayload>
{
    public Guid EventId {get; init;}
    public DateTimeOffset OccurredAt {get; init;}        
    public required Meta Meta {get; init;}
    public TPayload Payload { get; init; } = default!;
}