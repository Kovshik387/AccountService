using System.Collections.Immutable;
using System.Data;
using AccountService.Domain.Events;
using AccountService.Domain.Models;
using AccountService.Domain.Repositories;
using AccountService.Infrastructure.Common;
using AccountService.Infrastructure.Settings;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace AccountService.Infrastructure.Repositories;

public class OutboxRepository : PgRepository, IOutboxRepository
{
    public OutboxRepository(IOptions<DbOptions> options) : base(options)
    {
    }

    public async Task AddEventAsync<TPayload>(EventShell<TPayload> eventShell, string json, string routeKey,
        NpgsqlConnection connection, IDbTransaction transaction, CancellationToken cancellationToken)
    {
        var sql = @"
insert into outbox_messages (id, occurred_at, routing_key, data)
values (@Id, @OccurredAt, @RoutingKey, CAST(@Data AS jsonb));";

        var cmd = new CommandDefinition(sql, new
            {
                Id = eventShell.EventId,
                eventShell.OccurredAt,
                RoutingKey = routeKey,
                Data = json
            },
            transaction,
            cancellationToken: cancellationToken
        );

        await connection.ExecuteAsync(cmd);
    }

    public async Task DeleteEventAsync(NpgsqlConnection connection, IDbTransaction transaction, Guid[] ids,
        CancellationToken cancellationToken)
    {
        const string sql =
            "delete from outbox_messages where id = any(@Ids);";

        var cmd = new CommandDefinition(sql, new
            {
                Ids = ids
            },
            transaction: transaction,
            cancellationToken: cancellationToken
        );

        await connection.ExecuteAsync(cmd);
    }

    public async Task<IReadOnlyCollection<OutboxModel>> GetEventsBatchAsync(int batchSize,
        CancellationToken cancellationToken)
    {
        const string sql = @"
select id, routing_key, data::text as data, attempt_count
  from outbox_messages
 where coalesce(next_attempt_at, now()) <= now()
 order by occurred_at
 limit @batch;";

        await using var connection = await OpenConnectionAsync(cancellationToken);
        var result = await connection.QueryAsync<OutboxModel>(
            new CommandDefinition(
                sql,
                new
                {
                    batch = batchSize
                },
                cancellationToken: cancellationToken));
        return result.ToImmutableList();
    }

    public async Task<long> PendingCountAsync(CancellationToken cancellationToken)
    {
        const string sql = @"
select count(*) 
  from outbox_messages 
 where coalesce(next_attempt_at, now()) <= now();";

        var cmd = new CommandDefinition(
            sql,
            cancellationToken: cancellationToken
        );

        await using var connection = await OpenConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<long>(cmd);
    }

    public async Task ScheduleAsync(Guid id, TimeSpan delay, string error, CancellationToken ct)
    {
        const string sql = @"
update outbox_messages
   set next_attempt_at = now() + make_interval(secs => @sec),
       last_error      = left(@error, 4000)
 where id = @id;";

        await using var conn = await OpenConnectionAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(sql, new { id, sec = (int)delay.TotalSeconds, error },
            cancellationToken: ct));
    }
}