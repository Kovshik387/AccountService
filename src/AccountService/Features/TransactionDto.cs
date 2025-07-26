using AccountService.Domain.Entities.Enums;

namespace AccountService.Features;

public record TransactionDto
{
    public required Guid Id { get; init; }
    public required Guid AccountId { get; init; }
    public required TransactionType Type { get; init; }
    public Guid? CounterpartyAccountId { get; init; }
    public required decimal Amount { get; init; }
    public required string Currency { get; init; }
    public required string Description { get; init; }
    public required DateTimeOffset Date { get; init; }
}