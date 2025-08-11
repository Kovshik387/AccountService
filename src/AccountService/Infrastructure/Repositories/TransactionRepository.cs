using System.Data;
using System.Text;
using AccountService.Domain.Entities;
using AccountService.Domain.Repositories;
using AccountService.Infrastructure.Common;
using AccountService.Infrastructure.Settings;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace AccountService.Infrastructure.Repositories;

public class TransactionRepository : PgRepository, ITransactionRepository
{
    public TransactionRepository(IOptions<DbOptions> options) : base(options) { }
    
    public async Task<Guid> CreateTransactionAsync(Transaction transaction, NpgsqlConnection? dbConnection,
        IDbTransaction? dbTransaction, CancellationToken cancellationToken)
    {
        const string sql = @"
            insert into transactions (id, account_id, type, counterparty_account_id, amount, currency, description, date)
            values (@Id, @AccountId, @Type, @CounterpartyAccountId, @Amount, @Currency, @Description, @Date)
        ";
        
        var cmd = new CommandDefinition(
            sql,
            new
            {
                transaction.Id,
                transaction.AccountId,
                transaction.Type,
                transaction.CounterpartyAccountId,
                transaction.Amount,
                transaction.Currency,
                transaction.Description,
                transaction.Date
            },
            transaction: dbTransaction,
            cancellationToken: cancellationToken
        );
        
        if (dbConnection is not null)
        {
            await dbConnection.ExecuteAsync(cmd);
            return transaction.Id;
        }

        await using var connection = await OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(cmd);
        return transaction.Id;
    }

    public async Task<Transaction?> GetTransactionByIdAsync(Guid transactionId, CancellationToken cancellationToken)
    {
        const string sql = @"
            select * from transactions where id = @Id
        ";

        await using var connection = await OpenConnectionAsync(cancellationToken);
        var cmd = new CommandDefinition(
            sql,
            new
            {
                Id = transactionId
            }, 
            cancellationToken: cancellationToken
        );

        return await connection.QueryFirstOrDefaultAsync<Transaction>(cmd);
    }

    public async Task<IReadOnlyCollection<Transaction>> GetTransactionsAsync(Guid accountId, DateTimeOffset? from,
        DateTimeOffset? to, CancellationToken cancellationToken)
    {
        var sql = new StringBuilder(@"
            select * from transactions
            where account_id = @AccountId
        ");

        if (from != null)
            sql.Append(" and date >= @From");

        if (to != null)
            sql.Append(" and date <= @To");

        sql.Append(" order by date desc");

        await using var connection = await OpenConnectionAsync(cancellationToken);
        var cmd = new CommandDefinition(
            sql.ToString(),
            new
            {
                AccountId = accountId,
                From = from,
                To = to
            },
            cancellationToken: cancellationToken
        );

        var result = await connection.QueryAsync<Transaction>(cmd);
        return result.ToList().AsReadOnly();
    }
}