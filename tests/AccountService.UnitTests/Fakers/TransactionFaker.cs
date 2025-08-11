using AccountService.Domain.Entities;
using AccountService.Domain.Entities.Enums;
using Bogus;

namespace AccountService.UnitTests.Fakers;

public static class TransactionFaker
{
    private static readonly Faker<Transaction> Faker = new Faker<Transaction>()
        .RuleFor(x => x.Id, f => f.Random.Guid())
        .RuleFor(x => x.AccountId, f => f.Random.Guid())
        .RuleFor(x => x.Amount, f => f.Random.Decimal(0, 20))
        .RuleFor(x => x.Currency, f => f.PickRandom("RUB", "USD", "EUR"))
        .RuleFor(x => x.Description, f => f.Lorem.Sentence())
        .RuleFor(x => x.Date, f => f.Date.Past())
        .RuleFor(x => x.Type, f => f.PickRandom(TransactionType.Credit, TransactionType.Debit))
        ;

    public static Transaction[] Generate(int count = 1)
    {
        return Faker.Generate(count).ToArray();
    }
}