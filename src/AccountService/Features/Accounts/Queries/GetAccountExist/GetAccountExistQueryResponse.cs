namespace AccountService.Features.Accounts.Queries.GetAccountExist;

// ReSharper disable once NotAccessedPositionalProperty.Global Решарпер думает, что свойство не используется
/// <summary>
/// Результат запроса информации о существовании счёта 
/// </summary>
/// <param name="Exist">Булева переменная true - существует, false - нет</param>
public record GetAccountExistQueryResponse(bool Exist);