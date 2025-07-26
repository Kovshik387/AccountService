using FluentValidation;

namespace AccountService.Features.Accounts.Commands.UpdateAccount;

public class UpdateAccountCommandValidator : AbstractValidator<UpdateAccountCommand>
{
    public UpdateAccountCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");
        
        RuleFor(x => x.InterestRate)
            .NotNull()
            .GreaterThan(0)
            .WithMessage("Interest rate must be greater than zero");
    }
}