using MediatR;

namespace AccountService.Features.Accounts.Queries.GetAccountDetailsById;

public record GetAccountDetailsByIdQuery : IRequest<GetAccountDetailsByIdQueryResponse>
{
    public required Guid Id { get; init; }
    public DateTimeOffset? DateFrom { get; init; }
    public DateTimeOffset? DateTo { get; init; }
}