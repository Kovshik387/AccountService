using AccountService.Domain.Entities;
using AccountService.Domain.Entities.Enums;
using AccountService.Domain.Repositories;
using AccountService.Features.Exceptions;
using FluentValidation;
using MediatR;

namespace AccountService.Features.Transactions.Commands.TransferTransaction;

public class TransferTransactionCommandHandler : IRequestHandler<TransferTransactionCommand,
    TransferTransactionResponse>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;

    public TransferTransactionCommandHandler(IAccountRepository accountRepository, 
        ITransactionRepository transactionRepository)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
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
        
        _ = await _transactionRepository.CreateTransactionAsync(ownerTransaction, cancellationToken);
        _ = await _transactionRepository.CreateTransactionAsync(receiverTransaction, cancellationToken);
        
        owner = owner with { Balance = owner.Balance - request.Amount };
        receiver = receiver with { Balance = receiver.Balance + request.Amount };
        
        _ = await _accountRepository.UpdateBalanceAccountAsync(owner, cancellationToken);
        _ = await _accountRepository.UpdateBalanceAccountAsync(receiver, cancellationToken);
        
        return new TransferTransactionResponse(ownerTransaction.Id);
    }
}