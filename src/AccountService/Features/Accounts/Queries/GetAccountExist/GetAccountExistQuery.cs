using MediatR;

namespace AccountService.Features.Accounts.Queries.GetAccountExist;

public record GetAccountExistQuery(Guid AccountId, Guid OwnerId) : IRequest<GetAccountExistQueryResponse>;