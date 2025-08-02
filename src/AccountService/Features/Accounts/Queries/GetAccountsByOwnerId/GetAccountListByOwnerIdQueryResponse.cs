using AccountService.Domain.Entities.Enums;

namespace AccountService.Features.Accounts.Queries.GetAccountsByOwnerId;
/// <summary>
/// Информации о счетах
/// </summary>
public record GetAccountListByOwnerIdQueryResponse
{
    public Guid Id { get; init; }
    public required AccountType Type { get; init; }
    public required string Volute { get; init; }
    public required decimal Balance { get; init; }
    public decimal? InterestRate { get; init; }
    public required DateTimeOffset OpeningDate { get; init; }
    public DateTimeOffset? ClosingDate { get; init; }
}