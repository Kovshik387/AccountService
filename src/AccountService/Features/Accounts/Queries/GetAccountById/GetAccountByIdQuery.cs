using MediatR;

namespace AccountService.Features.Accounts.Queries.GetAccountById;
/// <summary>
/// Запрос на получение информации о счете
/// </summary>
/// <param name="Id">Уникальный идентификатор</param>
public record GetAccountByIdQuery(Guid Id) : IRequest<GetAccountByIdQueryResponse>;