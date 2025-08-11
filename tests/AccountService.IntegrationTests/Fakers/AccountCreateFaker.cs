using AccountService.Domain.Entities.Enums;
using AccountService.Features.Accounts.Commands.CreateAccount;
using Bogus;

namespace AccountService.IntegrationTests.Fakers;

public static class AccountCreateFaker
{
    private static readonly Faker<CreateAccountCommand> Faker = new Faker<CreateAccountCommand>()
        .RuleFor(x => x.OwnerId, f => f.Random.Guid())
        .RuleFor(x => x.Balance, f => f.Random.Decimal(min: 0, max: 1000))
        .RuleFor(x => x.InterestRate, f => f.Random.Decimal(min: 0, max: 100))
        .RuleFor(x => x.Currency, f => f.PickRandom("RUB", "USD", "EUR"))
        .RuleFor(x => x.Type, f => f.PickRandom(AccountType.Credit, AccountType.Checking));

    public static CreateAccountCommand[] Generate(int count = 1)
    {
        return Faker.Generate(count).ToArray();
    }

    public static CreateAccountCommand WithType(this CreateAccountCommand command, AccountType accountType) =>
        command with { Type = accountType };

    public static CreateAccountCommand WithBalance(this CreateAccountCommand command, decimal balance) =>
        command with { Balance = balance };

    public static CreateAccountCommand WithInterestRate(this CreateAccountCommand command, decimal? interestRate) =>
        command with { InterestRate = interestRate };

    public static CreateAccountCommand WithCurrency(this CreateAccountCommand command, string currency) =>
        command with { Currency = currency };
}