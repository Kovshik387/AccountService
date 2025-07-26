using AccountService.Features.Interfaces;
using FluentValidation;

namespace AccountService.Features.Transactions.Commands.TransferTransaction;

public class TransferTransactionCommandValidator : AbstractValidator<TransferTransactionCommand>
{
    public TransferTransactionCommandValidator(ICurrencyVerificationService currencyVerificationService)
    {
        RuleFor(x => x.OwnerAccountId)
            .NotNull()
            .NotEmpty()
            .WithMessage("The owner account id is required.");
        
        RuleFor(x => x.ReceiverAccountId)
            .NotNull()
            .NotEmpty()
            .WithMessage("The receiver account id is required.");
        
        RuleFor(x => x.Amount)
            .NotNull()
            .GreaterThan(0)
            .WithMessage("The amount must be greater than 0.");

        RuleFor(x => x.Currency)
            .NotNull()
            .NotEmpty()
            .MustAsync(currencyVerificationService.VerifyCurrencyAsync)
            .WithMessage("The currency does not exist.");
    }
}