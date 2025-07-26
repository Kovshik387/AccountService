using AccountService.Domain.Entities.Enums;
using MediatR;

namespace AccountService.Features.Transactions.Commands.CreateTransaction;
/// <summary>
/// Команда для создания трансфера между счетами
/// </summary>
public record CreateTransactionCommand : IRequest<CreateTransactionResponse>
{
    public CreateTransactionCommand(Guid accountId, Guid? counterpartyAccountId, TransactionType type,
        decimal amount, string currency, string description)
    {
        AccountId = accountId;
        CounterpartyAccountId = counterpartyAccountId;
        Type = type;
        Amount = amount;
        Currency = currency;
        Description = description;
    }

    /// <summary>
    /// Уникальный идентификатор счёта
    /// </summary>
    public Guid AccountId { get;  }
    /// <summary>
    /// Идентификатор контрагента
    /// </summary>
    public Guid? CounterpartyAccountId { get; }
    /// <summary>
    /// Тип перевода
    /// </summary>
    public TransactionType Type { get; }
    /// <summary>
    /// Сумма
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    /// Валюта ISO4217
    /// </summary>
    public string Currency { get; }
    /// <summary>
    /// Описание
    /// </summary>
    public string Description { get; }
}