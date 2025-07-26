using AccountService.Features.Interfaces;
using FluentValidation;

namespace AccountService.Features.Transactions.Commands.CreateTransaction;

public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator(ICurrencyVerificationService currencyVerificationService)
    {
        RuleFor(x => x.Currency)
            .MustAsync(currencyVerificationService.VerifyCurrencyAsync)
            .WithMessage("Unsupported currency");
        
        RuleFor(x => x.AccountId)
            .NotNull()
            .NotEmpty()
            .WithMessage("Invalid account");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Invalid amount");

        RuleFor(x => x.Type)
            .NotNull()
            .NotEmpty()
            .WithMessage("Invalid transaction type");
    }
}