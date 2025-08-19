using AccountService.Domain.Entities.Enums;

namespace AccountService.Domain.Entities;

public record Account
{
    public Guid Id { get; init; }
    public required Guid OwnerId { get; init; }
    public required AccountType Type { get; init; }
    public required string Currency { get; init; }
    public required decimal Balance { get; init; }
    public decimal? InterestRate { get; init; }
    public required DateTimeOffset OpeningDate { get; init; }
    public DateTimeOffset? ClosingDate { get; init; }
    public bool Frozen { get; init; } 
    public IReadOnlyCollection<Transaction> Transactions { get; init; } = [];
}