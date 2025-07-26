using MediatR;

namespace AccountService.Features.Accounts.Queries.GetAccountsByOwnerId;

public record GetAccountListByOwnerIdQuery(Guid OwnerId) : IRequest<IReadOnlyCollection<GetAccountListByOwnerIdQueryResponse>>;