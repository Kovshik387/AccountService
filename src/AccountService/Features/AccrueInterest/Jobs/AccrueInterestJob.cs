using AccountService.Features.AccrueInterest.Commands;
using MediatR;

namespace AccountService.Features.AccrueInterest.Jobs;

public class AccrueInterestJob
{
    private readonly IMediator _mediator;

    public AccrueInterestJob(IMediator mediator) => _mediator = mediator;

    public async Task InvokeAsync() => await _mediator.Send(new AccrueInterestCommand());
}