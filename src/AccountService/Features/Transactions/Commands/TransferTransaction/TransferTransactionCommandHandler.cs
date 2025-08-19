using System.Data;
using System.Text.Json;
using AccountService.Domain.Entities;
using AccountService.Domain.Entities.Enums;
using AccountService.Domain.Events;
using AccountService.Domain.Events.Publish;
using AccountService.Domain.Repositories;
using AccountService.Features.Exceptions;
using AccountService.Features.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;

namespace AccountService.Features.Transactions.Commands.TransferTransaction;

public class
    TransferTransactionCommandHandler : IRequestHandler<TransferTransactionCommand,
    TransferTransactionResponse>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IOutboxRepository _outboxRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IPgRepository _pgRepository;

    private readonly RouteOptions _routeOptions;

    private readonly ILogger<TransferTransactionCommandHandler> _logger;

    public TransferTransactionCommandHandler(IAccountRepository accountRepository,
        ITransactionRepository transactionRepository, IPgRepository pgRepository,
        ILogger<TransferTransactionCommandHandler> logger, IOutboxRepository outboxRepository,
        IOptions<RouteOptions> routeSettings)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _pgRepository = pgRepository;
        _logger = logger;
        _outboxRepository = outboxRepository;
        _routeOptions = routeSettings.Value;
    }

    public async Task<TransferTransactionResponse> Handle(TransferTransactionCommand request,
        CancellationToken cancellationToken)
    {
        await using var connection = await _pgRepository.OpenConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Serializable,
            cancellationToken);
        try
        {
            var owner = await _accountRepository.GetAccountByIdAsync(request.OwnerAccountId, cancellationToken);
            var receiver = await _accountRepository.GetAccountByIdAsync(request.ReceiverAccountId, cancellationToken);

            if (owner is null || receiver is null)
                throw new AccountNotFoundException("One or more owners does not exist.");

            if (owner.Currency != request.Currency || receiver.Currency != request.Currency)
                throw new ValidationException("Different currencies");

            if (owner.Balance < request.Amount) throw new ValidationException("Insufficient funds");

            var dateTransfer = DateTimeOffset.UtcNow;

            if (owner.Frozen || receiver.Frozen)
            {
                throw new ConflictException("Blocked");
            }
            
            var ownerTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = request.OwnerAccountId,
                CounterpartyAccountId = request.ReceiverAccountId,
                Amount = request.Amount,
                Currency = request.Currency,
                Type = TransactionType.Debit,
                Description = request.Description,
                Date = dateTransfer
            };

            var receiverTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = request.ReceiverAccountId,
                CounterpartyAccountId = request.OwnerAccountId,
                Amount = request.Amount,
                Currency = request.Currency,
                Type = TransactionType.Credit,
                Description = request.Description,
                Date = dateTransfer
            };

            var operationId = Guid.NewGuid();

            var ownerEvent = EventFactory.New(new MoneyDebited
                {
                    Currency = request.Currency,
                    Amount = request.Amount,
                    Reason = request.Description,
                    AccountId = owner.Id,
                    OperationId = operationId
                },
                request.CorrelationId,
                request.CausationId
            );

            var receiverEvent = EventFactory.New(
                new MoneyCredited
                {
                    Currency = request.Currency,
                    Amount = request.Amount,
                    AccountId = receiver.Id,
                    OperationId = operationId
                },
                request.CorrelationId,
                request.CausationId
            );

            var transferCompleteEvent = EventFactory.New(
                new TransferCompleted
                {
                    Currency = request.Currency,
                    OperationId = operationId,
                    Amount = request.Amount,
                    TransferId = Guid.NewGuid(),
                    DestinationAccountId = owner.Id,
                    SourceAccountId = receiver.Id,
                },
                request.CorrelationId,
                request.CausationId
            );

            _ = await _transactionRepository.CreateTransactionAsync(ownerTransaction, connection, transaction,
                cancellationToken);
            _ = await _transactionRepository.CreateTransactionAsync(receiverTransaction, connection, transaction,
                cancellationToken);

            var total = owner.Balance + receiver.Balance;

            var ownerBalance =
                await _accountRepository.UpdateBalanceAccountAsync(owner, connection, transaction, cancellationToken);
            var receiverBalance = await _accountRepository.UpdateBalanceAccountAsync(receiver, connection, transaction,
                cancellationToken);

            var expected = ownerBalance + receiverBalance;

            if (Math.Abs(total - expected) > 0.0001m)
                throw new AccountException("Balance mismatch — rolling back.");

            await _outboxRepository.AddEventAsync(ownerEvent, JsonSerializer.Serialize(ownerEvent),
                _routeOptions.DebitEventRoute, connection, transaction, cancellationToken);

            await _outboxRepository.AddEventAsync(receiverEvent, JsonSerializer.Serialize(receiverEvent),
                _routeOptions.CreditEventRoute, connection, transaction, cancellationToken);

            await _outboxRepository.AddEventAsync(transferCompleteEvent,
                JsonSerializer.Serialize(transferCompleteEvent),
                _routeOptions.TransferEventRoute, connection, transaction, cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return new TransferTransactionResponse(ownerTransaction.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transfer failed {CorrelationId}", request.CorrelationId);
            await transaction.RollbackAsync(cancellationToken);
            throw new ConflictException();
        }
    }
}