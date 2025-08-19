// ReSharper disable UnusedAutoPropertyAccessor.Global Получение свойств
namespace AccountService.Domain.Events.Publish;

public record TransferCompleted
{
    public Guid SourceAccountId { get; init; }
    public Guid DestinationAccountId { get; init; }
    public decimal Amount { get; init; }
    public required string Currency { get; init; }
    public Guid TransferId { get; init; }
    public Guid OperationId { get; init; }
}