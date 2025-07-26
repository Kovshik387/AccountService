using FluentValidation;

namespace AccountService.Features.Accounts.Queries.GetAccountExist;

public class GetAccountExistQueryValidator : AbstractValidator<GetAccountExistQuery>
{
    public GetAccountExistQueryValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty()
            .WithMessage("Account ID is required");
        
        RuleFor(x => x.OwnerId)
            .NotEmpty()
            .WithMessage("Owner ID is required");
    }
}