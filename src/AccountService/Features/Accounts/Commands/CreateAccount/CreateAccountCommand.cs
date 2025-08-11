using AccountService.Domain.Entities.Enums;
using MediatR;

namespace AccountService.Features.Accounts.Commands.CreateAccount;
/// <summary>
/// Команда для создания счёта
/// </summary>
public record CreateAccountCommand : IRequest<CreateAccountResponse>
{    
    /// <summary>
    /// Идентификатор клиента
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public required Guid OwnerId { get; init; }
    /// <summary>
    /// Тип счёта
    /// </summary>
    public required AccountType Type { get; init; }
    /// <summary>
    /// Валюта ISO4217
    /// </summary>
    public required string Currency { get; init; }
    /// <summary>
    /// Сумма
    /// </summary>
    public required decimal Balance { get; init; }
    /// <summary>
    /// Процентная ставка
    /// </summary>
    public decimal? InterestRate { get; init; }
}