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
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public required Guid Id { get; init; }
    /// <summary>
    /// Уникальный идентификатор счёта
    /// </summary>
    /// ReSharper disable once UnusedAutoPropertyAccessor.Global
    public required Guid AccountId { get; init; }
    /// <summary>
    /// Тип операции
    /// </summary>
    /// ReSharper disable once UnusedAutoPropertyAccessor.Global
    public required TransactionType Type { get; init; }
    /// <summary>
    /// Контрагент
    /// </summary>
    /// ReSharper disable once UnusedAutoPropertyAccessor.Global
    public Guid? CounterpartyAccountId { get; init; }
    /// <summary>
    /// Сумма
    /// </summary>
    /// ReSharper disable once UnusedAutoPropertyAccessor.Global
    public required decimal Amount { get; init; }
    /// <summary>
    /// Валюта ISO4217
    /// </summary>
    /// ReSharper disable once UnusedAutoPropertyAccessor.Global
    public required string Currency { get; init; }
    /// <summary>
    /// Описание
    /// </summary>
    /// ReSharper disable once UnusedAutoPropertyAccessor.Global
    public required string Description { get; init; }
    /// <summary>
    /// Дата и время проведения
    /// </summary>
    /// ReSharper disable once UnusedAutoPropertyAccessor.Global
    public required DateTimeOffset Date { get; init; }
}