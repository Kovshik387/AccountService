using MediatR;

namespace AccountService.Features.Accounts.Queries.GetAccountDetailsById;
/// <summary>
/// Запрос подробной информации о счёте
/// </summary>
public record GetAccountDetailsByIdQuery : IRequest<GetAccountDetailsByIdQueryResponse>
{
    /// <summary>
    /// Уникальный идентификатор
    /// </summary>
    public required Guid Id { get; init; }
    /// <summary>
    /// Первая дата
    /// </summary>
    public DateTimeOffset? DateFrom { get; init; }
    /// <summary>
    /// Вторая датаЫ
    /// </summary>
    public DateTimeOffset? DateTo { get; init; }
    /// <summary>
    /// Concurrency токен
    /// </summary>
    public uint XMin { get; init; }
}