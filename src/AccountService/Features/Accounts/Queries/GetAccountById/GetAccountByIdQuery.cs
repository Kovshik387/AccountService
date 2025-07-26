using MediatR;

namespace AccountService.Features.Accounts.Queries.GetAccountById;

public record GetAccountByIdQuery(Guid Id) : IRequest<GetAccountByIdQueryResponse>;