using AccountService.Domain.Repositories;
using AccountService.Features.Interfaces;
using AutoMapper;
using MediatR;

namespace AccountService.Features.Accounts.Queries.GetAccountsByOwnerId;

public class GetAccountListByOwnerIdQueryHandler : IRequestHandler<GetAccountListByOwnerIdQuery,
    IReadOnlyCollection<GetAccountListByOwnerIdQueryResponse>>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IClientVerificationService _clientVerificationService;
    private readonly ILogger<GetAccountListByOwnerIdQueryHandler> _logger;
    private readonly IMapper _mapper;

    public GetAccountListByOwnerIdQueryHandler(IAccountRepository accountRepository,
        ILogger<GetAccountListByOwnerIdQueryHandler> logger, IMapper mapper,
        IClientVerificationService clientVerificationService)
    {
        _accountRepository = accountRepository;
        _logger = logger;
        _mapper = mapper;
        _clientVerificationService = clientVerificationService;
    }

    public async Task<IReadOnlyCollection<GetAccountListByOwnerIdQueryResponse>> Handle(
        GetAccountListByOwnerIdQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var success = await _clientVerificationService.VerifyClientAsync(request.OwnerId);
        if (!success)
            throw new Exception("Client verification failed");
        
        var accounts = await _accountRepository.GetAccountsByIdOwnerAsync(request.OwnerId,
            cancellationToken);
        
        _logger.LogInformation("Get accounts by owner id: {RequestOwnerId}", request.OwnerId);
        
        return _mapper.Map<IReadOnlyCollection<GetAccountListByOwnerIdQueryResponse>>(accounts);
    }
}