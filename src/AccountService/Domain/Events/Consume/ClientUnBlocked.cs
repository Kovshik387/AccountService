namespace AccountService.Domain.Events.Consume;

public record ClientUnBlocked
{
    public Guid ClientId { get; init; }
}