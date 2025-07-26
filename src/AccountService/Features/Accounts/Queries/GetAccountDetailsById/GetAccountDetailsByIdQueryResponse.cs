using AccountService.Domain.Entities.Enums;

namespace AccountService.Features.Accounts.Queries.GetAccountDetailsById;

public record GetAccountDetailsByIdQueryResponse
{
    public Guid Id { get; init; }
    public required Guid OwnerId { get; init; }
    public required AccountType Type { get; init; }
    public required string Currency { get; init; }
    public required decimal Balance { get; init; }
    public decimal? InterestRate { get; init; }
    public required DateTimeOffset OpeningDate { get; init; }
    public DateTimeOffset? ClosingDate { get; init; }
    public required IReadOnlyCollection<TransactionDto> Transactions { get; init; }
}