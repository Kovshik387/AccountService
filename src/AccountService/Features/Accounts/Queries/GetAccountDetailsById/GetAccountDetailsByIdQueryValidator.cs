using FluentValidation;

namespace AccountService.Features.Accounts.Queries.GetAccountDetailsById;

public class GetAccountDetailsByIdQueryValidator : AbstractValidator<GetAccountDetailsByIdQuery>
{
    public GetAccountDetailsByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");

        When(x => x.DateFrom is not null && x.DateTo is not null, () =>
        {
            RuleFor(x => x)
                .Must(x => x.DateFrom!.Value <= x.DateTo!.Value)
                .WithMessage("Date from must be greater than DateTo");
        });
    }
}