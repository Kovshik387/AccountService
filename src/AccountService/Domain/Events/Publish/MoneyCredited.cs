// ReSharper disable UnusedAutoPropertyAccessor.Global Получение свойств
namespace AccountService.Domain.Events.Publish;

public record MoneyCredited
{
    public Guid AccountId { get; init; }
    public decimal Amount { get; init; }
    public required string Currency { get; init; }
    public Guid OperationId { get; init; }
}