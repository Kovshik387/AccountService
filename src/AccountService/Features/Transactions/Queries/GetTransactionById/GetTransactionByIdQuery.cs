using MediatR;

namespace AccountService.Features.Transactions.Queries.GetTransactionById;

public record GetTransactionByIdQuery : IRequest<GetTransactionByIdQueryResponse>
{
    public required Guid Id { get; init; }
}