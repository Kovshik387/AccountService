using System.Data;
using AccountService.Domain.Repositories;
using AccountService.Features.Interfaces;
using Dapper;
using MediatR;

namespace AccountService.Features.AccrueInterest.Commands;

public class AccrueInterestHandler : IRequestHandler<AccrueInterestCommand>
{
    private readonly ILogger<AccrueInterestHandler> _logger;
    
    private readonly IPgRepository _pgRepository;
    private readonly IAccountRepository _accountRepository;

    private const int ChunkSize = 10;
    private const string ProcedureName = "accrue_interest";
    
    public AccrueInterestHandler(IPgRepository pgRepository, IAccountRepository accountRepository, 
        ILogger<AccrueInterestHandler> logger)
    {
        _pgRepository = pgRepository;
        _accountRepository = accountRepository;
        _logger = logger;
    }

    public async Task Handle(AccrueInterestCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Accrue interest received");
        var ids = await _accountRepository.GetRatesAccounts(cancellationToken);
        
        await using var connection = await _pgRepository.OpenConnectionAsync(cancellationToken);
        
        foreach (var items in ids.Chunk(ChunkSize))
        {
            await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Serializable,
                cancellationToken);
            
            try
            {
                await connection.ExecuteAsync(
                    $"CALL {ProcedureName}(@Ids)",
                    new
                    {
                        Ids = ids
                    },
                    transaction
                );
                await transaction.CommitAsync(cancellationToken);
                _logger.LogInformation($"Accrue interest executed. {items.Length}");
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogInformation("Accrue interest failed.");
            }
        }
    }
}