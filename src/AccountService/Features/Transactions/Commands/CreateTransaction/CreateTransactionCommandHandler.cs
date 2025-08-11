using AccountService.Domain.Entities;
using AccountService.Domain.Entities.Enums;
using AccountService.Domain.Repositories;
using AccountService.Features.Exceptions;
using AccountService.Features.Interfaces;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace AccountService.Features.Transactions.Commands.CreateTransaction;

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, CreateTransactionResponse>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IPgRepository _pgRepository;
    private readonly IMapper _mapper;

    public CreateTransactionCommandHandler(IAccountRepository accountRepository,
        ITransactionRepository transactionRepository, IMapper mapper, IPgRepository pgRepository)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _mapper = mapper;
        _pgRepository = pgRepository;
    }

    public async Task<CreateTransactionResponse> Handle(CreateTransactionCommand request,
        CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetAccountByIdAsync(request.AccountId, cancellationToken);
        if (account is null) throw new AccountNotFoundException($"Account {request.AccountId} not found");

        if (account.Balance < request.Amount && request.Type == TransactionType.Debit)
            throw new ValidationException($"Failed to create {request.Type} transaction");
        
        var transaction = new Transaction
        {
            AccountId = request.AccountId,
            Amount = request.Amount,
            Type = request.Type,
            Description = request.Description,
            Date = DateTimeOffset.Now,
            Id = Guid.NewGuid(),
            Currency = request.Currency,
            CounterpartyAccountId = request.CounterpartyAccountId
        };

        await using var connection = await _pgRepository.OpenConnectionAsync(cancellationToken);
        await using var transactionDb = await connection.BeginTransactionAsync(cancellationToken);
        
        var result = await _transactionRepository.CreateTransactionAsync(transaction, connection, transactionDb,
            cancellationToken);

        var resultMoney = request.Type switch
        {
            TransactionType.Debit => -request.Amount,
            TransactionType.Credit => request.Amount,
            _ => 0
        };

        account = account with { Balance = resultMoney };
        await _accountRepository.UpdateBalanceAccountAsync(account, connection, transactionDb, cancellationToken);
        
        return _mapper.Map<CreateTransactionResponse>(result);
    }
}