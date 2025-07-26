using MediatR;

namespace AccountService.Features.Accounts.Commands.DeleteAccount;
/// <summary>
/// Команда для удаления счёта
/// </summary>
/// <param name="Id">Уникальный идентификатор счёта</param>
public record DeleteAccountCommand(Guid Id) : IRequest<DeleteAccountResponse>;