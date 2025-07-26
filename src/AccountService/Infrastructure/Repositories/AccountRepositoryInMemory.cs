using System.Collections.Concurrent;
using AccountService.Domain.Entities;
using AccountService.Domain.Models;
using AccountService.Domain.Repositories;

namespace AccountService.Infrastructure.Repositories;

public class AccountRepositoryInMemory : IAccountRepository
{
    private readonly ConcurrentDictionary<Guid, Account> _accounts = [];
    
    public Task<Guid?> AddAccountAsync(Account account, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var success = _accounts.TryAdd(account.Id, account);

        return Task.FromResult<Guid?>(success ? account.Id : null);
    }

    public async Task<Guid?> UpdateAccountAsync(UpdateAccountModel updateAccount, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var account = await GetAccountByIdAsync(updateAccount.Id, cancellationToken);
        if (account == null) return null;
        
        account = account with
        {
            InterestRate = updateAccount.InterestRate
        };
        
        _accounts[updateAccount.Id] = account;
        
        return account.Id;
    }

    public async Task<Guid> UpdateBalanceAccountAsync(Account updateAccount, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _accounts[updateAccount.Id] = updateAccount;
        
        return await Task.FromResult(updateAccount.Id);
    }

    public Task<Guid?> DeleteAccountAsync(Guid accountId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var success = _accounts.TryRemove(accountId, out _);

        return Task.FromResult<Guid?>(success ? accountId : null);
    }

    public async Task<IReadOnlyCollection<Account>> GetAccountsByIdOwnerAsync(Guid ownerId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var accounts = _accounts.Values.Where(x => x.OwnerId.Equals(ownerId))
            .ToList()
            .AsReadOnly();
        
        return await Task.FromResult(accounts);
    }

    public Task<Account?> GetAccountByIdAsync(Guid accountId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        _ = _accounts.TryGetValue(accountId, out var account);
        
        return Task.FromResult(account);
    }

    public Task<bool> AccountExistsAsync(Guid accountId, Guid ownerId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = _accounts[accountId].OwnerId.Equals(ownerId);
        return Task.FromResult(result);
    }
}