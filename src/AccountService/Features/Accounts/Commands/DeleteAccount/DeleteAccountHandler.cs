using AccountService.Domain.Repositories;
using AccountService.Features.Exceptions;
using MediatR;

namespace AccountService.Features.Accounts.Commands.DeleteAccount;

public class DeleteAccountHandler : IRequestHandler<DeleteAccountCommand, DeleteAccountResponse>
{
    private readonly IAccountRepository _repository;
    private readonly ILogger<DeleteAccountHandler> _logger;

    public DeleteAccountHandler(IAccountRepository repository, ILogger<DeleteAccountHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<DeleteAccountResponse> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var id = await _repository.DeleteAccountAsync(request.Id, cancellationToken);
        
        if (id is null) throw new AccountNotFoundException("Account not found");
        
        _logger.LogInformation("Account with id {Guid} has been deleted", id);

        return new DeleteAccountResponse(id);
    }
}