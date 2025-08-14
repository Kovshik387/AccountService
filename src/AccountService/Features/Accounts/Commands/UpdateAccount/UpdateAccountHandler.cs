using AccountService.Domain.Models;
using AccountService.Domain.Repositories;
using AccountService.Features.Exceptions;
using AutoMapper;
using MediatR;

namespace AccountService.Features.Accounts.Commands.UpdateAccount;

public class UpdateAccountHandler : IRequestHandler<UpdateAccountCommand, UpdateAccountResponse>
{
    private readonly IAccountRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateAccountHandler> _logger;

    public UpdateAccountHandler(IAccountRepository repository, ILogger<UpdateAccountHandler> logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<UpdateAccountResponse> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var idAccount = await _repository.UpdateAccountAsync(_mapper.Map<UpdateAccountModel>(request),
            cancellationToken);

        if (idAccount == null)
        {
            var exist = await _repository.GetAccountByIdAsync(request.Id, cancellationToken);
            if (exist is null) throw new AccountNotFoundException("Account not found");
            
            throw new ConflictException("Account conflict");
        }
        
        _logger.LogInformation("Update InterestRate id - {idAccount}", idAccount);
        return new UpdateAccountResponse(idAccount);
    }
}