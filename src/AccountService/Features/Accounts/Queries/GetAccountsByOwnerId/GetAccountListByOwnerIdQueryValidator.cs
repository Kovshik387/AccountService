using FluentValidation;

namespace AccountService.Features.Accounts.Queries.GetAccountsByOwnerId;

public class GetAccountListByOwnerIdQueryValidator : AbstractValidator<GetAccountListByOwnerIdQuery>
{
    public GetAccountListByOwnerIdQueryValidator()
    {
        RuleFor(x => x.OwnerId)
            .NotEmpty()
            .WithMessage("OwnerId is required");
    }    
}