using System.Transactions;
using AccountService.Features.Interfaces;
using AccountService.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Npgsql;

namespace AccountService.Infrastructure.Common;

public class PgRepository : IPgRepository
{
    private readonly DbOptions _options;

    // ReSharper disable once MemberCanBeProtected.Global не может инжектнуть
    public PgRepository(IOptions<DbOptions> options)
    {
        _options = options.Value;
    }

    public async Task<NpgsqlConnection> OpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        if (Transaction.Current is not null &&
            Transaction.Current.TransactionInformation.Status is TransactionStatus.Aborted)
        {
            throw new TransactionAbortedException("Transaction was aborted (probably by user cancellation request)");
        }
        
        var connection = new NpgsqlConnection(_options.ConnectionString);
        await connection.OpenAsync(cancellationToken);
        
        await connection.ReloadTypesAsync(cancellationToken);
        
        return connection;
    }
}