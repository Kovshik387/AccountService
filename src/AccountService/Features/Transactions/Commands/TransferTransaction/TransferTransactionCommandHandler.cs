using System.Data;
using AccountService.Domain.Entities;
using AccountService.Domain.Entities.Enums;
using AccountService.Domain.Repositories;
using AccountService.Features.Exceptions;
using AccountService.Features.Interfaces;
using FluentValidation;
using MediatR;

namespace AccountService.Features.Transactions.Commands.TransferTransaction;

public class 
    TransferTransactionCommandHandler : IRequestHandler<TransferTransactionCommand,
    TransferTransactionResponse>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IPgRepository _pgRepository;

    private readonly ILogger<TransferTransactionCommandHandler> _logger;
    
    public TransferTransactionCommandHandler(IAccountRepository accountRepository, 
        ITransactionRepository transactionRepository, IPgRepository pgRepository,
        ILogger<TransferTransactionCommandHandler> logger)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _pgRepository = pgRepository;
        _logger = logger;
    }

    public async Task<TransferTransactionResponse> Handle(TransferTransactionCommand request,
        CancellationToken cancellationToken)
    {
        var owner = await _accountRepository.GetAccountByIdAsync(request.OwnerAccountId, cancellationToken);
        var receiver = await _accountRepository.GetAccountByIdAsync(request.ReceiverAccountId, cancellationToken);
        
        if (owner is null || receiver is null) throw new AccountNotFoundException("One or more owners does not exist.");

        if (owner.Currency != request.Currency || receiver.Currency != request.Currency)
            throw new ValidationException("Different currencies");

        if (owner.Balance < request.Amount) throw new ValidationException("Insufficient funds");
        
        var dateTransfer = DateTimeOffset.UtcNow;
        
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

        await using var connection = await _pgRepository.OpenConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Serializable, 
            cancellationToken);
        try
        {
            _ = await _transactionRepository.CreateTransactionAsync(ownerTransaction, connection, transaction,
                cancellationToken);
            _ = await _transactionRepository.CreateTransactionAsync(receiverTransaction, connection, transaction,
                cancellationToken);

            owner = owner with { Balance = owner.Balance - request.Amount };
            receiver = receiver with { Balance = receiver.Balance + request.Amount };

            _ = await _accountRepository.UpdateBalanceAccountAsync(owner, connection, transaction, cancellationToken);
            _ = await _accountRepository.UpdateBalanceAccountAsync(receiver, connection, transaction,
                cancellationToken);

            var total = owner.Balance + receiver.Balance;
            var expected = (owner.Balance + request.Amount) + (receiver.Balance - request.Amount);

            if (Math.Abs(total - expected) > 0.0001m)
                throw new AccountException("Balance mismatch — rolling back.");
            
            await transaction.CommitAsync(cancellationToken);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.Message);
            await transaction.RollbackAsync(cancellationToken);
            throw new ConflictException();
        }
        
        return new TransferTransactionResponse(ownerTransaction.Id);
    }
}