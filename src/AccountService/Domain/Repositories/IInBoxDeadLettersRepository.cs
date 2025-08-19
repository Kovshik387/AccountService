namespace AccountService.Domain.Repositories;

public interface IInBoxDeadLettersRepository
{
    public Task AddAsync(Guid messageId, string handler, string json, string error, CancellationToken cancellationToken);
}