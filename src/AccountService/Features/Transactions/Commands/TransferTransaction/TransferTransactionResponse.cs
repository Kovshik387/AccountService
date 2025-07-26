namespace AccountService.Features.Transactions.Commands.TransferTransaction;
/// <summary>
/// Результат трансфера
/// </summary>
/// <param name="Id">Уникальный идентификатор</param>
public record TransferTransactionResponse(Guid Id);