namespace AccountService.Domain.Events.Publish;

public record MoneyDebited
{
    public Guid AccountId { get; init; }
    public decimal Amount { get; init; }
    public required string Currency { get; init; }
    public Guid OperationId { get; init; }
    public required string Reason { get; init; }
}