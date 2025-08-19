namespace AccountService.Domain.Models;

public record OutboxModel
{
    public Guid Id { get; init; }
    public string RoutingKey { get; init; } = string.Empty;
    public string Data { get; init; } = string.Empty;
    public int AttemptCount { get; init; }
}