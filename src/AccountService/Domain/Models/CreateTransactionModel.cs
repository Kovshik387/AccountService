using AccountService.Domain.Entities.Enums;

namespace AccountService.Domain.Models;

public class CreateTransactionModel
{
    public required Guid Id { get; init; }
    public required Guid AccountId { get; init; }
    public required TransactionType Type { get; init; }
    public Guid? CounterpartyAccountId { get; init; }
    public required decimal Amount { get; init; }
    public required string Volute { get; init; }
    public required string Description { get; init; }
}