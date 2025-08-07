namespace AccountService.Features.Accounts.Commands.UpdateAccount;

// ReSharper disable once NotAccessedPositionalProperty.Global Решарпер думает, что свойство не используется
/// <summary>
/// Информация об счёте
/// </summary>
/// <param name="Id">Уникальный идентификатор</param>
public record UpdateAccountResponse(Guid? Id);