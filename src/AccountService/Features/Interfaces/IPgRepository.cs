using Npgsql;

namespace AccountService.Features.Interfaces;

public interface IPgRepository
{
    public Task<NpgsqlConnection> OpenConnectionAsync(CancellationToken cancellationToken = default);
}