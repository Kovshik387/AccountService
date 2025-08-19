using System.Net;
using System.Net.Http.Json;
using AccountService.Features;
using AccountService.Features.Accounts.Queries.GetAccountById;
using AccountService.IntegrationTests.Creators;
using AccountService.IntegrationTests.Fixtures;
using FluentAssertions;

namespace AccountService.IntegrationTests;

[Collection(nameof(FixtureDefinition))]
public class ParallelTransferTests
{
    private readonly HttpClient _client;
    
    private const string TransactionPath = "/api/transactions/transfer";

    public ParallelTransferTests(ApiFixture<Program> fixture)
    {
        _client = fixture.Client;
    }

    [Theory]
    [InlineData(50)]
    public async Task TransfersTransaction_Should_SaveTotal(int testCount)
    {
        // Arrange
        const decimal balance = 1000;
        const string currency = "RUB";
        var firstAccount = await CreateAccount.CreateAccountAsync(_client, balance, currency);
        var secondAccount = await CreateAccount.CreateAccountAsync(_client, balance, currency);

        const decimal sum = balance * 2;
        var body = new
        {
            OwnerAccountId = firstAccount,
            ReceiverAccountId = secondAccount,
            Amount = balance * 0.1m,
            Currency = currency,
            Description = "Parallel test"
        };

        // Act
        var tasks = Enumerable.Range(0, testCount)
            .Select(_ => _client.PostAsJsonAsync(TransactionPath, body));

        var responses = await Task.WhenAll(tasks);

        var unexpected = responses.Where(x => !x.IsSuccessStatusCode
                                              && x.StatusCode != HttpStatusCode.Conflict).ToList();

        // Assert
        var totalAfter = await GetTotalAsync((Guid)firstAccount!, (Guid)secondAccount!);
        unexpected.Should().BeEmpty();
        totalAfter.Should().Be(sum);
    }


    private async Task<decimal> GetTotalAsync(Guid first, Guid second)
    {
        var firstResponse =
            await _client.GetFromJsonAsync<MbResult<GetAccountByIdQueryResponse>>(CreateAccount.AccountPath + first);
        var secondResponse =
            await _client.GetFromJsonAsync<MbResult<GetAccountByIdQueryResponse>>(CreateAccount.AccountPath + second);
        return firstResponse!.Result!.Balance + secondResponse!.Result!.Balance;
    }
}