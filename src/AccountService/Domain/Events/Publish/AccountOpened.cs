using AccountService.Domain.Entities.Enums;

namespace AccountService.Domain.Events.Publish;

public record AccountOpened
{
    public Guid AccountId { get; init; }
    public Guid OwnerId { get; init; }
    public required string Currency { get; init; }
    public AccountType AccountType { get; init; }
}