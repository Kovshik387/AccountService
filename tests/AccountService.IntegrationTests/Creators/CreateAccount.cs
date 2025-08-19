using System.Net.Http.Json;
using AccountService.Domain.Entities.Enums;
using AccountService.Features;
using AccountService.Features.Accounts.Commands.CreateAccount;
using AccountService.IntegrationTests.Fakers;

namespace AccountService.IntegrationTests.Creators;

public static class CreateAccount
{
    public const string AccountPath = "/api/accounts/";

    public static async Task<Guid?> CreateAccountAsync(HttpClient client ,decimal balance, string currency)
    {
        var response = await client.PostAsJsonAsync(AccountPath,
            AccountCreateFaker.Generate().First()
                .WithBalance(balance)
                .WithType(AccountType.Checking)
                .WithInterestRate(null)
                .WithCurrency(currency)
        );
        
        response.EnsureSuccessStatusCode();

        var dto = await response.Content.ReadFromJsonAsync<MbResult<CreateAccountResponse>>();
        return dto!.Result!.Id;
    }
}