using Npgsql;

namespace AccountService.Domain.Repositories;

public interface IInBoxRepository
{
    Task<bool> MarkProcessingAsync(Guid messageId, string name, NpgsqlConnection connection,
        NpgsqlTransaction transaction, CancellationToken cancellationToken);
}