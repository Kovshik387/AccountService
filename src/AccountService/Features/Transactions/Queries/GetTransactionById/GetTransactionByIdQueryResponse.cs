using AccountService.Domain.Entities.Enums;

namespace AccountService.Features.Transactions.Queries.GetTransactionById;
/// <summary>
/// Получение транзакции
/// </summary>
public record GetTransactionByIdQueryResponse
{
    /// <summary>
    /// Уникальный идентификатор
    /// </summary>
    public required Guid Id { get; init; }
    /// <summary>
    /// Уникальный идентификатор счёта
    /// </summary>
    public required Guid AccountId { get; init; }
    /// <summary>
    /// Тип операции
    /// </summary>
    public required TransactionType Type { get; init; }
    /// <summary>
    /// Контрагент
    /// </summary>
    public Guid? CounterpartyAccountId { get; init; }
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
    public required string Description { get; init; }
    /// <summary>
    /// Дата и время проведения
    /// </summary>
    public required DateTimeOffset Date { get; init; }
}