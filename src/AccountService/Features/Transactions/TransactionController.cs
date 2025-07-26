using AccountService.Features.Transactions.Commands.CreateTransaction;
using AccountService.Features.Transactions.Commands.TransferTransaction;
using AccountService.Features.Transactions.Queries.GetTransactionById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.Transactions;

[ApiController]
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
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(CreateTransaction), new { id = result.Id }, null);
    }
    /// <summary>
    /// Получить транзакцию по Id
    /// </summary>
    /// <param name="id">Уникальный идентификатор транзакции</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetTransactionById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTransactionByIdQuery { Id = id }, cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Произвести трансфер между счетами
    /// </summary>
    /// <param name="command">Тело запроса</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost]
    [Route("transfer")]
    public async Task<IActionResult> TransferTransaction([FromBody] TransferTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}