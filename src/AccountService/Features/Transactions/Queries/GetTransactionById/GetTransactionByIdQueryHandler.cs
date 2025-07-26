using AccountService.Domain.Repositories;
using AccountService.Features.Exceptions;
using AutoMapper;
using MediatR;

namespace AccountService.Features.Transactions.Queries.GetTransactionById;

public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, GetTransactionByIdQueryResponse>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMapper _mapper;
    
    public GetTransactionByIdQueryHandler(ITransactionRepository transactionRepository, IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    public async Task<GetTransactionByIdQueryResponse> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _transactionRepository.GetTransactionByIdAsync(request.Id, cancellationToken);

        if (result is null) throw new TransactionNotFoundException("Failed to get transaction by id");
        
        return _mapper.Map<GetTransactionByIdQueryResponse>(result);
    }
}