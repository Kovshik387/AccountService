using AccountService.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace AccountService.Features.Accounts.Queries.GetAccountById;

public class GetAccountByIdQueryHandler : IRequestHandler<GetAccountByIdQuery, GetAccountByIdQueryResponse>
{
    private readonly IAccountRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAccountByIdQueryHandler> _logger;

    public GetAccountByIdQueryHandler(IAccountRepository repository, IMapper mapper,
        ILogger<GetAccountByIdQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetAccountByIdQueryResponse> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
    {
        var account = await _repository.GetAccountByIdAsync(request.Id, cancellationToken);
        _logger.LogInformation("Get account id : {account.Id}", account?.Id);
        
        return _mapper.Map<GetAccountByIdQueryResponse>(account);
    }
}