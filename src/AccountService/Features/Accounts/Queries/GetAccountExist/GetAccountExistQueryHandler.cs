using AccountService.Domain.Repositories;
using MediatR;

namespace AccountService.Features.Accounts.Queries.GetAccountExist;

public class GetAccountExistQueryHandler : IRequestHandler<GetAccountExistQuery, GetAccountExistQueryResponse>
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountExistQueryHandler(IAccountRepository accountRepository) => _accountRepository = accountRepository;

    public async Task<GetAccountExistQueryResponse> Handle(GetAccountExistQuery request, CancellationToken cancellationToken)
    {
        return new GetAccountExistQueryResponse(
            await _accountRepository.AccountExistsAsync(request.AccountId, request.OwnerId, cancellationToken));
    }
}