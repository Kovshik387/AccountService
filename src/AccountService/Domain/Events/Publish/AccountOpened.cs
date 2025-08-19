using AccountService.Domain.Entities.Enums;
// ReSharper disable UnusedAutoPropertyAccessor.Global Получение свойств

namespace AccountService.Domain.Events.Publish;
/// <summary>
/// Открытие аккаунта
/// </summary>
public record AccountOpened
{
    public Guid AccountId { get; init; }
    public Guid OwnerId { get; init; }
    public required string Currency { get; init; }
    public AccountType AccountType { get; init; }
}