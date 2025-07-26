using MediatR;

namespace AccountService.Features.Transactions.Commands.TransferTransaction;
/// <summary>
/// Команда для создания трансфера между счетами
/// </summary>
public record TransferTransactionCommand : IRequest<TransferTransactionResponse>
{
    public TransferTransactionCommand(Guid ownerAccountId, Guid receiverAccountId, decimal amount, string currency,
        string description)
    {
        OwnerAccountId = ownerAccountId;
        ReceiverAccountId = receiverAccountId;
        Amount = amount;
        Currency = currency;
        Description = description;
    }

    /// <summary>
    /// Уникальный идентификатор агента
    /// </summary>
    public required Guid OwnerAccountId { get; init; }
    /// <summary>
    /// Уникальный идентификатор контрагента
    /// </summary>
    public required Guid ReceiverAccountId { get; init; }
    /// <summary>
    /// Сумма
    /// </summary>
    public required decimal Amount { get; init; }
    /// <summary>
    /// Валюта ISO4217
    /// </summary>
    public required string Currency { get; init; }
    /// <summary>
    /// Описание
    /// </summary>
    public string Description { get; init; } = string.Empty;
}