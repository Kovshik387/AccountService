using System.Collections.Concurrent;
using AccountService.Domain.Entities;
using AccountService.Domain.Repositories;

namespace AccountService.Infrastructure.Repositories;

public class TransactionRepositoryInMemory : ITransactionRepository
{
    private readonly ConcurrentDictionary<Guid, Transaction> _transactions = [];
    
    public Task<Guid> CreateTransactionAsync(Transaction transaction, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        _ = _transactions.TryAdd(transaction.Id, transaction);
        
        return Task.FromResult(transaction.Id);
    }

    public Task<Transaction?> GetTransactionByIdAsync(Guid transactionId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _ = _transactions.TryGetValue(transactionId, out var transactionModel);
        return Task.FromResult(transactionModel);
    }

    public async Task<IReadOnlyCollection<Transaction>> GetTransactionsAsync(Guid accountId, DateTimeOffset? from, DateTimeOffset? to,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var transactions = _transactions.Values
            .Where(x => x.AccountId.Equals(accountId));
            
        if (from is not null)
            transactions = transactions.Where(x => x.Date >= from.Value);

        if (to is not null)
            transactions = transactions.Where(x => x.Date <= to.Value);

        return await Task.FromResult(transactions.ToList().AsReadOnly());
    }
}