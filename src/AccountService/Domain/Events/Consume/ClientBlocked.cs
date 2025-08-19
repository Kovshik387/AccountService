namespace AccountService.Domain.Events.Consume;

public record ClientBlocked
{
    public Guid ClientId { get; init; }
}