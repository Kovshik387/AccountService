using AccountService.Features.Accounts.Commands.UpdateAccount;
using Bogus;

namespace AccountService.UnitTests.Fakers;

public static class UpdateAccountCommandFaker
{
    private static readonly Faker<UpdateAccountCommand> Faker = new Faker<UpdateAccountCommand>()
        .RuleFor(u => u.Id, f => f.Random.Guid())
        .RuleFor(u => u.InterestRate, f => f.Random.Decimal(0,20))
        .RuleFor(u => u.XMin, f => f.Random.UInt(1,200))
        ;

    public static UpdateAccountCommand[] Generate(int count = 1)
    {
        return Faker.Generate(count).ToArray();
    }
    
}