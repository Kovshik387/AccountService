namespace AccountService.Features.Transactions.Commands.TransferTransaction;
/// <summary>
/// Результат трансфера
/// </summary>
/// <param name="Id">Уникальный идентификатор</param>
// ReSharper disable once NotAccessedPositionalProperty.Global Решарпер думает, что свойство не используется
public record TransferTransactionResponse(Guid Id);