using AccountService.Domain.Repositories;
using Dapper;
using Npgsql;

namespace AccountService.Infrastructure.Repositories;

public sealed class InBoxRepository : IInBoxRepository
{
    public async Task<bool> MarkProcessingAsync(Guid messageId, string name, NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        CancellationToken cancellationToken)
    {
        const string sql = @"
insert into inbox_consumed (message_id, handler, processed_at)
values (@MessageId, @Handler, now())
on conflict (message_id) do nothing;";

        var result = await connection.ExecuteAsync(
            new CommandDefinition(sql,
                new
                {
                    MessageId = messageId,
                    Handler = name
                },
                transaction,
                cancellationToken: cancellationToken)
        );
        
        return result == 1;
    }
}