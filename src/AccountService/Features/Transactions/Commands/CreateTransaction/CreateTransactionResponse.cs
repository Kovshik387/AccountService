namespace AccountService.Features.Transactions.Commands.CreateTransaction;
/// <summary>
/// Результат создания транзакции
/// </summary>
public record CreateTransactionResponse
{
    /// <summary>
    /// Уникальный идентификатор
    /// </summary>
    public required Guid Id { get; init; }
}