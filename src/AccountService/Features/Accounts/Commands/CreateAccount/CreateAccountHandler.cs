using System.Text.Json;
using AccountService.Converters;
using AccountService.Domain.Entities;
using AccountService.Domain.Events;
using AccountService.Domain.Events.Publish;
using AccountService.Domain.Repositories;
using AccountService.Features.Exceptions;
using AccountService.Features.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;

namespace AccountService.Features.Accounts.Commands.CreateAccount;

public class CreateAccountHandler : IRequestHandler<CreateAccountCommand, CreateAccountResponse>
{
    private readonly IAccountRepository _repository;
    private readonly IPgRepository _pgRepository;
    private readonly IOutboxRepository _outboxRepository;
    private readonly ILogger<CreateAccountHandler> _logger;

    private readonly RouteOptions _routeOptions;
    
    public CreateAccountHandler(IAccountRepository repository, ILogger<CreateAccountHandler> logger,
        IOutboxRepository outboxRepository, IPgRepository pgRepository, IOptions<RouteOptions> routeSettings)
    {
        _repository = repository;
        _logger = logger;
        _outboxRepository = outboxRepository;
        _pgRepository = pgRepository;
        _routeOptions = routeSettings.Value;
    }

    public async Task<CreateAccountResponse> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = new Account
        {
            Balance = request.Balance,
            Type = request.Type,
            OwnerId = request.OwnerId,
            Currency = request.Currency,
            OpeningDate = DateTimeOffset.UtcNow,
            Id = Guid.NewGuid(),
            InterestRate = request.InterestRate
        };

        await using var connection = await _pgRepository.OpenConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            var response = await _repository.AddAccountAsync(account, connection, transaction, cancellationToken);

            var eventShel = EventFactory.New(new AccountOpened()
            {
                AccountId = account.Id,
                Currency = request.Currency,
                OwnerId = account.OwnerId,
                AccountType = account.Type,
            },
                request.CorrelationId,
                request.CausationId
            );

            await _outboxRepository.AddEventAsync(
                eventShel,
                JsonSerializer.Serialize(eventShel, JsonOption.Options),
                _routeOptions.AccountOpenEventRoute,
                connection,
                transaction,
                cancellationToken
            );

            _logger.LogInformation("Adding account with id {response}", response);

            if (response == null)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new AccountException("Could not add account");
            }

            await transaction.CommitAsync(cancellationToken);

            return new CreateAccountResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await transaction.RollbackAsync(cancellationToken);
            throw new AccountException("Could not add account");
        }
    }
}