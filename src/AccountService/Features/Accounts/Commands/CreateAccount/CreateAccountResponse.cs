namespace AccountService.Features.Accounts.Commands.CreateAccount;

/// <summary>
/// Ответ после удаления account'а
/// </summary>
/// <param name="Id">Идентификатор удалённого account</param>
public record CreateAccountResponse(Guid? Id);