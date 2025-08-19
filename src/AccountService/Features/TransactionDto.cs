using AccountService.Domain.Entities.Enums;

namespace AccountService.Features;
/// <summary>
/// Предоставление операции
/// </summary>
public record TransactionDto
{
    /// <summary>
    /// Уникальный идентификатор
    /// </summary>
    public required Guid Id { get; init; }
    /// <summary>
    /// Идентификатор счёта
    /// </summary>
    public required Guid AccountId { get; init; }
    /// <summary>
    /// Тип транзакции
    /// </summary>
    public required TransactionType Type { get; init; }
    /// <summary>
    /// Контрагент
    /// </summary>
    public Guid? CounterpartyAccountId { get; init; }
    /// <summary>
    /// Сумма операции
    /// </summary>
    public required decimal Amount { get; init; }
    /// <summary>
    /// Валюта
    /// </summary>
    public required string Currency { get; init; }
    /// <summary>
    /// Описание
    /// </summary>
    public required string Description { get; init; }
    /// <summary>
    /// Дата
    /// </summary>
    public required DateTimeOffset Date { get; init; }
}