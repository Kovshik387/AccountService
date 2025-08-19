using System.Data;
using AccountService.Domain.Events;
using AccountService.Domain.Models;
using Npgsql;

namespace AccountService.Domain.Repositories;

public interface IOutboxRepository
{
    Task AddEventAsync<TPayload>(EventShell<TPayload> eventShell, string json, string routeKey,
        NpgsqlConnection connection, IDbTransaction transaction, CancellationToken cancellationToken);
    
    Task DeleteEventAsync(NpgsqlConnection connection, IDbTransaction transaction, Guid[] ids,
        CancellationToken cancellationToken);
    
    Task<IReadOnlyCollection<OutboxModel>> GetEventsBatchAsync(int batchSize, CancellationToken cancellationToken);
    
    Task<long> PendingCountAsync(CancellationToken cancellationToken);
    
    Task ScheduleAsync(Guid id, TimeSpan delay, string error, CancellationToken cancellationToken);
}