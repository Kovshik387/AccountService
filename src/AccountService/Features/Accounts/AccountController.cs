using AccountService.Features.Accounts.Commands.CreateAccount;
using AccountService.Features.Accounts.Commands.DeleteAccount;
using AccountService.Features.Accounts.Commands.UpdateAccount;
using AccountService.Features.Accounts.Queries.GetAccountById;
using AccountService.Features.Accounts.Queries.GetAccountDetailsById;
using AccountService.Features.Accounts.Queries.GetAccountExist;
using AccountService.Features.Accounts.Queries.GetAccountsByOwnerId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.Accounts;

[ApiController]
[Authorize]
[Route("api/accounts")]
public class AccountController : ControllerBase
{
    private readonly IMediator _mediator;
    
    // ReSharper disable once ConvertToPrimaryConstructor
    public AccountController(IMediator mediator) => _mediator = mediator;
    
    /// <summary>
    /// Создание пользовательского счёта
    /// </summary>
    /// <param name="command">Тело запроса</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(MbResult<CreateAccountResponse>),StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(MbResult), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(MbResult),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MbResult),StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(CreateAccount), new { result.Id },
            MbResult<CreateAccountResponse>.SuccessResult(result));
    }
    /// <summary>
    /// Получение счёта по id
    /// </summary>
    /// <param name="id">Уникальный идентификатор счёта</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(MbResult<GetAccountByIdQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MbResult),StatusCodes.Status400BadRequest)]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetAccountById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAccountByIdQuery(id), cancellationToken);
        return Ok(MbResult<GetAccountByIdQueryResponse>.SuccessResult(result));
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
    [ProducesResponseType(typeof(MbResult<GetAccountDetailsByIdQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MbResult), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(MbResult),StatusCodes.Status400BadRequest)]
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
        
        return Ok(MbResult<GetAccountDetailsByIdQueryResponse>.SuccessResult(result));
    }
    /// <summary>
    /// Проверка существования счёта
    /// </summary>
    /// <param name="id">Уникальный идентификатор счёта</param>
    /// <param name="ownerId">Уникальный идентификатор клиенты</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(MbResult<GetAccountExistQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MbResult),StatusCodes.Status400BadRequest)]
    [Route("{id:guid}/exists")]
    public async Task<IActionResult> GetAccountExist(Guid id, [FromQuery] Guid ownerId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAccountExistQuery(id, ownerId), cancellationToken);
        return Ok(MbResult<GetAccountExistQueryResponse>.SuccessResult(result));
    }
    /// <summary>
    /// Получение всех счетов пользователя
    /// </summary>
    /// <param name="ownerId">Уникальный идентификатор пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(MbResult<IReadOnlyCollection<GetAccountListByOwnerIdQueryResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MbResult),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MbResult), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(MbResult), StatusCodes.Status500InternalServerError)]
    [Route("owner/{ownerId:guid}")]
    public async Task<IActionResult> GetAccountsByOwnerId(Guid ownerId, CancellationToken cancellationToken)
    {
        var result = await _mediator
            .Send(new GetAccountListByOwnerIdQuery(ownerId), cancellationToken);
        return Ok(MbResult<IReadOnlyCollection<GetAccountListByOwnerIdQueryResponse>>.SuccessResult(result));
    }
    /// <summary>
    /// Обновление ставки счёта
    /// </summary>
    /// <param name="id">Уникальный идентификатор счёта</param>
    /// <param name="command">Тело запроса</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    [HttpPatch]
    [ProducesResponseType(typeof(MbResult<UpdateAccountResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MbResult),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MbResult), StatusCodes.Status409Conflict)]
    [Route("{id:guid}")]
    public async Task<IActionResult> UpdateAccount(Guid id, [FromBody] UpdateAccountCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(MbResult<UpdateAccountResponse>.SuccessResult(result));
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
    [ProducesResponseType(typeof(MbResult<DeleteAccountResponse>),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MbResult), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(MbResult),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MbResult),StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAccount(Guid id, [FromBody] DeleteAccountCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(MbResult<DeleteAccountResponse>.SuccessResult(result));
    }
}