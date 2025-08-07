using MediatR;

namespace AccountService.Features.Accounts.Queries.GetAccountsByOwnerId;
/// <summary>
/// Запрос счётов по владельцу
/// </summary>
/// <param name="OwnerId">Уникальный идентификатор пользователя</param>
public record GetAccountListByOwnerIdQuery(Guid OwnerId) : IRequest<IReadOnlyCollection<GetAccountListByOwnerIdQueryResponse>>;