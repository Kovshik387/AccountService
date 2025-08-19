using System.Net.Http.Json;
using System.Text.Json;
using AccountService.Domain.Events;
using AccountService.Domain.Events.Consume;
using AccountService.Domain.Events.Publish;
using AccountService.Features;
using AccountService.IntegrationTests.Creators;
using AccountService.IntegrationTests.Fixtures;
using FluentAssertions;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using AccountService.Features.Outbox.Jobs;
using AccountService.Features.Interfaces;

namespace AccountService.IntegrationTests;

[Collection(nameof(FixtureDefinition))]
public class EventsTests
{
    private readonly ApiFixture<Program> _fixture;
    private readonly HttpClient _client;

    private const string TransactionPath = "/api/transactions/transfer";
    
    public EventsTests(ApiFixture<Program> fixture)
    {
        _fixture = fixture;
        _client = fixture.Client;
    }

    [Fact]
    public async Task TransfersTransactionBlocked_Should_ThrowConflict()
    {
        // Arrange
        using var scope = _fixture.Services.CreateScope();
        var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
        var routes = scope.ServiceProvider.GetRequiredService<IOptions<RouteOptions>>().Value;

        var antifraudConsumer = scope.ServiceProvider.GetRequiredService<AntifraudConsumer>();
        await antifraudConsumer.RunAsync(JobCancellationToken.Null);

        const decimal balance = 1000;
        const string currency = "RUB";
        
        var firstAccount = await CreateAccount.CreateAccountAsync(_client, balance, currency);
        firstAccount.Should().NotBeNull();
        var secondAccount = await CreateAccount.CreateAccountAsync(_client, balance, currency);
        
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        var moneyDebitedTcs =
            new TaskCompletionSource<(string queue, string json)>(TaskCreationOptions.RunContinuationsAsynchronously);
        await messageBus.Subscribe<EventShell<MoneyDebited>>(routes.DebitEventRoute, msg =>
        {
            var json = JsonSerializer.Serialize(msg);
            moneyDebitedTcs.TrySetResult((routes.DebitEventRoute, json));
            return Task.CompletedTask;
        }, cts.Token);

        // Act
        var blockedShell = EventFactory.New(new ClientBlocked
        {
            ClientId = firstAccount.Value
        });

        await messageBus.PushAsync(routes.ClientBlockEventRoute, blockedShell, cts.Token);

        var body = new
        {
            OwnerAccountId = firstAccount,
            ReceiverAccountId = secondAccount,
            Amount = balance * 0.1m,
            Currency = currency,
            Description = "Parallel test"
        };

        // Act
        var response = await _client.PostAsJsonAsync(TransactionPath, body, cancellationToken: cts.Token);
        
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);

        var didTimeout = await WaitNoEvent(moneyDebitedTcs.Task, TimeSpan.FromSeconds(3), cts.Token);
        didTimeout.Should().BeTrue("MoneyDebited must not be published for blocked accounts");
    }

    [Fact]
    public async Task CreateAccount_Should_PushEvent()
    {
        // Arrange
        await _fixture.RabbitMqContainer.PauseAsync();

        var accountId = await CreateAccount.CreateAccountAsync(_client, 1000m, "RUB");
        accountId.Should().NotBeNull();

        await _fixture.RabbitMqContainer.UnpauseAsync();

        using var scope = _fixture.Services.CreateScope();
        var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
        var routes = scope.ServiceProvider.GetRequiredService<IOptions<RouteOptions>>().Value;

        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        var result = new TaskCompletionSource<(string queue, string json)>(TaskCreationOptions
            .RunContinuationsAsynchronously);

        // Act
        await messageBus.Subscribe<EventShell<AccountOpened>>(routes.AccountOpenEventRoute, message =>
        {
            var serialize = JsonSerializer.Serialize(message);
            result.TrySetResult((routes.AccountOpenEventRoute, serialize));

            return Task.CompletedTask;
        }, cancellationTokenSource.Token);

        var publisher = scope.ServiceProvider.GetRequiredService<OutboxPublisherJob>();
        await publisher.RunAsync(JobCancellationToken.Null, batchSize: 50);

        // Assert
        var (queue, json) = await WaitFor(result, cancellationTokenSource.Token);
        queue.Should().Be(routes.AccountOpenEventRoute);
        json.Should().NotBeNullOrWhiteSpace();
    }

    private static async Task<T> WaitFor<T>(TaskCompletionSource<T> task, CancellationToken cancellationToken)
    {
        using (cancellationToken.Register(() => task.TrySetCanceled(cancellationToken)))
        {
            return await task.Task.ConfigureAwait(false);
        }
    }

    private static async Task<bool> WaitNoEvent(Task task, TimeSpan timeout, CancellationToken ct)
    {
        var delayTask = Task.Delay(timeout, ct);
        var completed = await Task.WhenAny(task, delayTask);
        return completed == delayTask;
    }
}