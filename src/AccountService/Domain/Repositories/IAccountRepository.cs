using System.Data;
using AccountService.Domain.Entities;
using AccountService.Domain.Models;
using Npgsql;

namespace AccountService.Domain.Repositories;

public interface IAccountRepository
{
    Task<Guid?> AddAccountAsync(Account account, NpgsqlConnection dbConnection, IDbTransaction dbTransaction,
        CancellationToken cancellationToken);

    Task<Guid?> UpdateAccountAsync(UpdateAccountModel updateAccount, CancellationToken cancellationToken);

    Task<decimal> UpdateBalanceAccountAsync(Account account, NpgsqlConnection dbConnection,
        IDbTransaction dbTransaction,
        CancellationToken cancellationToken);

    Task<Guid?> DeleteAccountAsync(Guid accountId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Account>> GetAccountsByIdOwnerAsync(Guid ownerId, CancellationToken cancellationToken);
    Task<Account?> GetAccountByIdAsync(Guid accountId, CancellationToken cancellationToken);
    Task<bool> AccountExistsAsync(Guid accountId, Guid ownerId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Guid>> GetRatesAccounts(CancellationToken cancellationToken);

    Task<Guid?> BlockAccountAsync(Guid accountId, NpgsqlConnection dbConnection, IDbTransaction dbTransaction,
        CancellationToken cancellationToken);

    Task<Guid?> UnblockAccountAsync(Guid accountId, NpgsqlConnection dbConnection, IDbTransaction dbTransaction,
        CancellationToken cancellationToken);
}