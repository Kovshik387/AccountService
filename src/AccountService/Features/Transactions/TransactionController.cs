using AccountService.Features.Transactions.Commands.CreateTransaction;
using AccountService.Features.Transactions.Commands.TransferTransaction;
using AccountService.Features.Transactions.Queries.GetTransactionById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.Transactions;

[ApiController]
[Authorize]
[Route("api/transactions")]
public class TransactionController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransactionController(IMediator mediator) => _mediator = mediator;
    /// <summary>
    /// Создать транзакцию
    /// </summary>
    /// <param name="command">Тело запроса</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(MbResult<CreateTransactionResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(MbResult),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MbResult), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(CreateTransaction), new { id = result.Id },
            MbResult<CreateTransactionResponse>.SuccessResult(result));
    }
    /// <summary>
    /// Получить транзакцию по Id
    /// </summary>
    /// <param name="id">Уникальный идентификатор транзакции</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    [ProducesResponseType(typeof(MbResult<GetTransactionByIdQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MbResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MbResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(MbResult), StatusCodes.Status409Conflict)]
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetTransactionById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTransactionByIdQuery { Id = id },
            cancellationToken);
        return Ok(MbResult<GetTransactionByIdQueryResponse>.SuccessResult(result));
    }
    /// <summary>
    /// Произвести трансфер между счетами
    /// </summary>
    /// <param name="command">Тело запроса</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    [ProducesResponseType(typeof(MbResult<TransferTransactionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MbResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(MbResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MbResult), StatusCodes.Status409Conflict)]
    [HttpPost]
    [Route("transfer")]
    public async Task<IActionResult> TransferTransaction([FromBody] TransferTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(MbResult<TransferTransactionResponse>.SuccessResult(result));
    }
}