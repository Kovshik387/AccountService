using System.Data;
using AccountService.Domain.Entities;
using AccountService.Domain.Models;
using AccountService.Domain.Repositories;
using AccountService.Infrastructure.Common;
using AccountService.Infrastructure.Settings;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace AccountService.Infrastructure.Repositories;

public class AccountRepository : PgRepository, IAccountRepository
{
    public AccountRepository(IOptions<DbOptions> options) : base(options)
    {
    }

    public async Task<Guid?> AddAccountAsync(Account account, NpgsqlConnection dbConnection,
        IDbTransaction dbTransaction,
        CancellationToken cancellationToken)
    {
        const string sql = @"
       insert into accounts (id, owner_id, type, currency, balance, interest_rate, opening_date)
       values (@Id, @OwnerId, @Type, @Currency, @Balance, @InterestRate, @OpeningDate)
";

        var cmd = new CommandDefinition(
            sql,
            new
            {
                account.Id,
                account.OwnerId,
                account.Type,
                account.Currency,
                account.Balance,
                account.InterestRate,
                account.OpeningDate
            },
            cancellationToken: cancellationToken
        );

        var success = await dbConnection.ExecuteAsync(cmd);
        return success > 0 ? account.Id : null;
    }

    public async Task<Guid?> UpdateAccountAsync(UpdateAccountModel updateAccount, CancellationToken cancellationToken)
    {
        const string sql = @"
       update accounts
          set interest_rate = @InterestRate
        where id = @Id and xmin = @XMin
        ";

        await using var connection = await OpenConnectionAsync(cancellationToken);
        var cmd = new CommandDefinition(
            sql,
            new
            {
                updateAccount.InterestRate,
                updateAccount.Id
            },
            cancellationToken: cancellationToken
        );

        var success = await connection.ExecuteAsync(cmd);
        return success > 0 ? updateAccount.Id : null;
    }

    public async Task<decimal> UpdateBalanceAccountAsync(Account account, NpgsqlConnection dbConnection,
        IDbTransaction dbTransaction,
        CancellationToken cancellationToken)
    {
        const string sql = @"
            update accounts
            set balance = @Balance
            where id = @
            returning balance
        ";

        var cmd = new CommandDefinition(
            sql,
            new
            {
                account.Id,
                account.Balance
            },
            cancellationToken: cancellationToken
        );

        return await dbConnection.ExecuteScalarAsync<decimal>(cmd);
    }

    public async Task<Guid?> DeleteAccountAsync(Guid accountId, CancellationToken cancellationToken)
    {
        const string sql = @"
       delete from accounts where id = @Id
        ";

        await using var connection = await OpenConnectionAsync(cancellationToken);
        var cmd = new CommandDefinition(
            sql,
            new
            {
                Id = accountId
            },
            cancellationToken: cancellationToken
        );

        var success = await connection.ExecuteAsync(cmd);
        return success > 0 ? accountId : null;
    }

    public async Task<IReadOnlyCollection<Account>> GetAccountsByIdOwnerAsync(Guid ownerId,
        CancellationToken cancellationToken)
    {
        const string sql = @"
            select * from accounts where owner_id = @OwnerId
        ";

        await using var connection = await OpenConnectionAsync(cancellationToken);
        var cmd = new CommandDefinition(
            sql,
            new
            {
                OwnerId = ownerId
            },
            cancellationToken: cancellationToken
        );

        var result = await connection.QueryAsync<Account>(cmd);
        return result.ToList().AsReadOnly();
    }

    public async Task<Account?> GetAccountByIdAsync(Guid accountId, CancellationToken cancellationToken)
    {
        const string sql = @"
            select id, owner_id, type, currency, balance, interest_rate, opening_date, closing_date, xmin
            from accounts where id = @Id
        ";

        await using var connection = await OpenConnectionAsync(cancellationToken);
        var cmd = new CommandDefinition(
            sql,
            new
            {
                Id = accountId
            },
            cancellationToken: cancellationToken
        );

        var result = await connection.QueryFirstOrDefaultAsync<Account>(cmd);
        return result;
    }

    public async Task<bool> AccountExistsAsync(Guid accountId, Guid ownerId, CancellationToken cancellationToken)
    {
        const string sql = @"
            select exists (
                select 1 from accounts where id = @Id and owner_id = @OwnerId
            )
        ";

        await using var connection = await OpenConnectionAsync(cancellationToken);
        var cmd = new CommandDefinition(
            sql,
            new
            {
                Id = accountId,
                OwnerId = ownerId
            },
            cancellationToken: cancellationToken
        );

        var exists = await connection.ExecuteScalarAsync<bool>(cmd);
        return exists;
    }

    public async Task<IReadOnlyCollection<Guid>> GetRatesAccounts(CancellationToken cancellationToken)
    {
        const string sql = @"
            select id from accounts where interest_rate is not null and interest_rate > 0  
        ";

        await using var connection = await OpenConnectionAsync(cancellationToken);
        var cmd = new CommandDefinition(
            sql,
            cancellationToken: cancellationToken
        );

        var result = await connection.QueryAsync<Guid>(cmd);
        return result.ToList().AsReadOnly();
    }

    public async Task<Guid?> BlockAccountAsync(Guid accountId, NpgsqlConnection dbConnection,
        IDbTransaction dbTransaction, CancellationToken cancellationToken)
    {
        const string sql = @"
update accounts set frozen = true where id = @Id";

        var cmd = new CommandDefinition(
            sql,
            new
            {
                Id = accountId,
            },
            transaction: dbTransaction,
            cancellationToken: cancellationToken);

        var success = await dbConnection.ExecuteAsync(cmd);
        return success > 0 ? accountId : null;
    }

    public async Task<Guid?> UnblockAccountAsync(Guid accountId, NpgsqlConnection dbConnection,
        IDbTransaction dbTransaction, CancellationToken cancellationToken)
    {
        const string sql = @"
update accounts set blocked = false where id = @Id";

        var cmd = new CommandDefinition(
            sql,
            new
            {
                Id = accountId,
            },
            transaction: dbTransaction,
            cancellationToken: cancellationToken);

        var success = await dbConnection.ExecuteAsync(cmd);
        return success > 0 ? accountId : null;
    }
}