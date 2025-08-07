using MediatR;

namespace AccountService.Features.Accounts.Queries.GetAccountExist;
/// <summary>
/// Запрос на существования счёта
/// </summary>
/// <param name="AccountId">Идентификатор счёта</param>
/// <param name="OwnerId">Идентификатор пользователя</param>
public record GetAccountExistQuery(Guid AccountId, Guid OwnerId) : IRequest<GetAccountExistQueryResponse>;