namespace AccountService.Domain.Entities.Enums;
/// <summary>
/// Перечисление типов транзакции
/// </summary>
public enum TransactionType : short
{
    Unknown = 0,
    Credit,
    Debit
}