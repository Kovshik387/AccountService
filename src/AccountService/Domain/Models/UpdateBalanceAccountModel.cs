using AccountService.Domain.Entities.Enums;

namespace AccountService.Domain.Models;

public class UpdateBalanceAccountModel
{
    public required Guid Id { get; init; }
    public required Guid OwnerId { get; init; }
    public required decimal Balance { get; init; }
    public required TransactionType TransactionType { get; init; }
}