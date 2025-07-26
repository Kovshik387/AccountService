using AccountService.Domain.Entities.Enums;
using AccountService.Features.Interfaces;
using FluentValidation;

namespace AccountService.Features.Accounts.Commands.CreateAccount;

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator(ICurrencyVerificationService currencyVerificationService)
    {
        RuleFor(x => x.OwnerId)
            .NotEmpty();
        
        RuleFor(x => x.Currency)
            .MustAsync(currencyVerificationService.VerifyCurrencyAsync)
            .WithMessage("Unsupported currency");
        
        RuleFor(x => x.Type)
            .NotEmpty()
            .WithMessage("Type is required");

        When(x => x.Type is AccountType.Deposit or AccountType.Credit, () =>
        {
            RuleFor(x => x.InterestRate)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Interest rate must be greater than zero");
        });
        
        When(x => x.Type == AccountType.Checking, () =>
        {
            RuleFor(x => x.InterestRate)
                .Null()
                .Empty()
                .WithMessage("Interest rate must be null");
        });
    }
} 