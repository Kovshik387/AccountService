using System.Data;
using AccountService.Domain.Entities;
using Npgsql;

namespace AccountService.Domain.Repositories;

public interface ITransactionRepository
{
    public Task<Guid> CreateTransactionAsync(Transaction transaction, NpgsqlConnection? dbConnection,
        IDbTransaction? dbTransaction, CancellationToken cancellationToken);
    Task<Transaction?> GetTransactionByIdAsync(Guid transactionId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Transaction>> GetTransactionsAsync(Guid accountId, DateTimeOffset? from,
        DateTimeOffset? to, CancellationToken cancellationToken);
}