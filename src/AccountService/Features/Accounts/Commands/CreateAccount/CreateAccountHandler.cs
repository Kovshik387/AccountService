using AccountService.Domain.Entities;
using AccountService.Domain.Repositories;
using AccountService.Features.Exceptions;
using MediatR;

namespace AccountService.Features.Accounts.Commands.CreateAccount;

public class CreateAccountHandler : IRequestHandler<CreateAccountCommand, CreateAccountResponse>
{
    private readonly IAccountRepository _repository;
    private readonly ILogger<CreateAccountHandler> _logger;
    
    public CreateAccountHandler(IAccountRepository repository, ILogger<CreateAccountHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<CreateAccountResponse> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = new Account
        {
            Balance = request.Balance,
            Type = request.Type,
            OwnerId = request.OwnerId,
            Currency = request.Currency,
            OpeningDate = DateTimeOffset.UtcNow,
            Id = Guid.NewGuid(),
            InterestRate = request.InterestRate
        };
        
        var response = await _repository.AddAccountAsync(account, cancellationToken);
        
        _logger.LogInformation("Adding account with id {response}", response);
        
        if (response == null) throw new AccountException("Could not add account");
        
        return new CreateAccountResponse(response);
    }
}