using AccountService.Domain.Repositories;
using AccountService.Infrastructure.Common;
using AccountService.Infrastructure.Settings;
using Dapper;
using Microsoft.Extensions.Options;

namespace AccountService.Infrastructure.Repositories;

public class InBoxDeadLettersRepository : PgRepository, IInBoxDeadLettersRepository
{
    public InBoxDeadLettersRepository(IOptions<DbOptions> options) : base(options)
    {
    }

    public async Task AddAsync(Guid messageId, string handler, string json, string error,
        CancellationToken cancellationToken)
    {
        const string sql = @"
insert into inbox_dead_letters (message_id, handler, payload, error)
values (@MessageId, @Handler, CAST(@Payload AS jsonb), @Error);";

        var cmd = new CommandDefinition(sql, new
            {
                MessageId = messageId,
                Handler = handler,
                Payload = json,
                Error = "Envelope/version validation failed"
            },
            cancellationToken: cancellationToken
        );

        await using var conn = await OpenConnectionAsync(cancellationToken);
        await conn.ExecuteAsync(cmd);
    }
}