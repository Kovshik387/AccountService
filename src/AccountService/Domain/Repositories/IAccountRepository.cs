using System.Data.Common;
using AccountService.Domain.Entities;
using AccountService.Domain.Models;
using Npgsql;

namespace AccountService.Domain.Repositories;

public interface IAccountRepository
{
    Task<Guid?> AddAccountAsync(Account account, CancellationToken cancellationToken);
    Task<Guid?> UpdateAccountAsync(UpdateAccountModel updateAccount, CancellationToken cancellationToken);
    Task<Guid> UpdateBalanceAccountAsync(Account account, NpgsqlConnection dbConnection, DbTransaction dbTransaction,
        CancellationToken cancellationToken);
    Task<Guid?> DeleteAccountAsync(Guid accountId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Account>> GetAccountsByIdOwnerAsync(Guid ownerId, CancellationToken cancellationToken);
    Task<Account?> GetAccountByIdAsync(Guid accountId, CancellationToken cancellationToken);
    Task<bool> AccountExistsAsync(Guid accountId, Guid ownerId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Guid>> GetRatesAccounts(CancellationToken cancellationToken);
}