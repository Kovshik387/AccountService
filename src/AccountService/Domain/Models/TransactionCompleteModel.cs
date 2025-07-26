using AccountService.Domain.Entities.Enums;

namespace AccountService.Domain.Models;

public record TransactionCompleteModel
{
    public Guid Id { get; init; }
    public TransactionType Type { get; init; }
    public DateTimeOffset Date { get; init; }
}