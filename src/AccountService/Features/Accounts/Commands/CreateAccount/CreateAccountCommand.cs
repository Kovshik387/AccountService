using AccountService.Domain.Entities.Enums;
using AccountService.Features.Interfaces;
using MediatR;

namespace AccountService.Features.Accounts.Commands.CreateAccount;
/// <summary>
/// Команда для создания счёта
/// </summary>
public record CreateAccountCommand : IRequest<CreateAccountResponse>, ICorrelationMessage
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
    /// <summary>
    /// Id сообщения
    /// </summary>
    public Guid MessageId { get; set; } = Guid.NewGuid();
    /// <summary>
    /// Корреляция 
    /// </summary>
    public string? CorrelationId { get; set; }
    /// <summary>
    /// Causation Id
    /// </summary>
    public string? CausationId { get; set; }
}