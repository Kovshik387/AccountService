namespace AccountService.Domain.Events.Consume;

public record ClientUnBlocked
{
    public ClientUnBlocked(Guid clientId)
    {
        ClientId = clientId;
    }

    public Guid ClientId { get; init; }
}