using AccountService.Domain.Entities;

namespace AccountService.Domain.Repositories;

public interface ITransactionRepository
{
    Task<Guid> CreateTransactionAsync(Transaction transaction, 
        CancellationToken cancellationToken);
    Task<Transaction?> GetTransactionByIdAsync(Guid transactionId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Transaction>> GetTransactionsAsync(Guid accountId, DateTimeOffset? from,
        DateTimeOffset? to, CancellationToken cancellationToken);
}