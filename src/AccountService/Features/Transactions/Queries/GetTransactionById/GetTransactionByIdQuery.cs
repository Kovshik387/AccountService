using MediatR;

namespace AccountService.Features.Transactions.Queries.GetTransactionById;
/// <summary>
/// Получение транзакции
/// </summary>
public record GetTransactionByIdQuery : IRequest<GetTransactionByIdQueryResponse>
{
    /// <summary>
    /// Уникальный идентификатор
    /// </summary>
    public required Guid Id { get; init; }
}