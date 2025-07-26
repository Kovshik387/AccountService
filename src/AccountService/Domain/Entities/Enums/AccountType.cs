namespace AccountService.Domain.Entities.Enums;
/// <summary>
/// Перечисление типов счёта
/// </summary>
public enum AccountType : short
{
    Unknown = 0,
    Checking,
    Deposit,
    Credit
}