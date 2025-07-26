using AccountService.Domain.Repositories;
using AccountService.Features.Exceptions;
using AutoMapper;
using MediatR;

namespace AccountService.Features.Accounts.Queries.GetAccountDetailsById;

public class GetAccountDetailsByIdQueryHandler : IRequestHandler<GetAccountDetailsByIdQuery,
    GetAccountDetailsByIdQueryResponse>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMapper _mapper;
    
    public GetAccountDetailsByIdQueryHandler(ITransactionRepository transactionRepository,
        IAccountRepository accountRepository, IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _accountRepository = accountRepository;
        _mapper = mapper;
    }

    public async Task<GetAccountDetailsByIdQueryResponse> Handle(GetAccountDetailsByIdQuery request,
        CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetAccountByIdAsync(request.Id, cancellationToken);
        if (account is null) throw new AccountNotFoundException("Account not found");
        
        var transactions = await _transactionRepository.GetTransactionsAsync(request.Id,
            request.DateFrom, request.DateTo, cancellationToken);

        account = account with { Transactions = transactions };
        
        return _mapper.Map<GetAccountDetailsByIdQueryResponse>(account);
    }
}