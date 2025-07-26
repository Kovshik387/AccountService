using AccountService.Features.Accounts.Commands.CreateAccount;
using AccountService.Features.Accounts.Commands.DeleteAccount;
using AccountService.Features.Accounts.Commands.UpdateAccount;
using AccountService.Features.Accounts.Queries.GetAccountById;
using AccountService.Features.Accounts.Queries.GetAccountDetailsById;
using AccountService.Features.Accounts.Queries.GetAccountExist;
using AccountService.Features.Accounts.Queries.GetAccountsByOwnerId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.Accounts;

[ApiController]
[Route("api/accounts")]
public class AccountController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public AccountController(IMediator mediator) => _mediator = mediator;
    
    /// <summary>
    /// Создание пользовательского счёта
    /// </summary>
    /// <param name="command">Тело запроса</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(CreateAccount), new { result.Id }, null);
    }
    /// <summary>
    /// Получение счёта по id
    /// </summary>
    /// <param name="id">Уникальный идентификатор счёта</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetAccountById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAccountByIdQuery(id), cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Получение транзакций пользователя по счёту
    /// </summary>
    /// <param name="id">Уникальный идентификатор счёта</param>
    /// <param name="from">Дата с</param>
    /// <param name="to">Дата по</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    [HttpGet]
    [Route("{id:guid}/transactions")]
    public async Task<IActionResult> GetAccountTransactions(Guid id, [FromQuery] DateTimeOffset from,
        [FromQuery] DateTimeOffset to, CancellationToken cancellationToken)
    {
        var result =  await _mediator.Send(new GetAccountDetailsByIdQuery
        {
            Id = id,
            DateFrom = from,
            DateTo = to
        }, cancellationToken);
        
        return Ok(result);
    }
    /// <summary>
    /// Проверка существования счёта
    /// </summary>
    /// <param name="id">Уникальный идентификатор счёта</param>
    /// <param name="ownerId">Уникальный идентификатор клиенты</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    [HttpGet]
    [Route("{id:guid}/exists")]
    public async Task<IActionResult> GetAccountExist(Guid id, [FromQuery] Guid ownerId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAccountExistQuery(id, ownerId), cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Получение всех счетов пользователя
    /// </summary>
    /// <param name="ownerId">Уникальный идентификатор пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    [HttpGet]
    [Route("owner/{ownerId:guid}")]
    public async Task<IActionResult> GetAccountsByOwnerId(Guid ownerId, CancellationToken cancellationToken)
    {
        var result = await _mediator
            .Send(new GetAccountListByOwnerIdQuery(ownerId), cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Обновление ставки счёта
    /// </summary>
    /// <param name="id">Уникальный идентификатор счёта</param>
    /// <param name="command">Тело запроса</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    [HttpPatch]
    [Route("{id:guid}")]
    public async Task<IActionResult> UpdateAccount(Guid id, [FromBody] UpdateAccountCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Удаление счёта
    /// </summary>
    /// <param name="id">Уникальный идентификатор счёта</param>
    /// <param name="command">Тело запроса</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    [HttpDelete]
    [Route("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAccount(Guid id, [FromBody] DeleteAccountCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}