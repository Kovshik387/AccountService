using MediatR;

namespace AccountService.Features.Accounts.Commands.UpdateAccount;
/// <summary>
/// Команда обновления процентной ставки
/// </summary>
public record UpdateAccountCommand : IRequest<UpdateAccountResponse>
{
    public UpdateAccountCommand(decimal interestRate, Guid id)
    {
        InterestRate = interestRate;
        Id = id;
    }

    /// <summary>
    /// Уникальный идентификатор
    /// </summary>
    public Guid Id { get; init; }
    /// <summary>
    /// Процентная ставка
    /// </summary>
    public decimal InterestRate { get; init; }
}