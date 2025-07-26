using AccountService.Domain.Entities;
using AccountService.Domain.Entities.Enums;
using AccountService.Domain.Repositories;
using AccountService.Features.Exceptions;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace AccountService.Features.Transactions.Commands.CreateTransaction;

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, CreateTransactionResponse>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMapper _mapper;

    public CreateTransactionCommandHandler(IAccountRepository accountRepository,
        ITransactionRepository transactionRepository, IMapper mapper)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _mapper = mapper;
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
        
        var result = await _transactionRepository.CreateTransactionAsync(transaction, cancellationToken);

        var resultMoney = request.Type switch
        {
            TransactionType.Debit => request.Amount,
            TransactionType.Credit => request.Amount,
            _ => 0
        };

        account = account with { Balance = resultMoney };
        await _accountRepository.UpdateBalanceAccountAsync(account, cancellationToken);
        
        return _mapper.Map<CreateTransactionResponse>(result);
    }
}