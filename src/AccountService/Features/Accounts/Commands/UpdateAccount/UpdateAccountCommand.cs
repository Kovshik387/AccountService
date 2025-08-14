using MediatR;

namespace AccountService.Features.Accounts.Commands.UpdateAccount;
/// <summary>
/// Команда обновления процентной ставки
/// </summary>
public record UpdateAccountCommand : IRequest<UpdateAccountResponse>
{
    public UpdateAccountCommand() { }
    public UpdateAccountCommand(decimal interestRate, Guid id, uint xMin)
    {
        InterestRate = interestRate;
        Id = id;
        XMin = xMin;
    }

    /// <summary>
    /// Уникальный идентификатор
    /// </summary>
    public Guid Id { get; init; }
    /// <summary>
    /// Процентная ставка
    /// </summary>
    public decimal InterestRate { get; init; }
    /// <summary>
    /// 
    /// </summary>
    public uint XMin { get; init; }
}