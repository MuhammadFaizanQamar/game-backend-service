using FluentValidation;
using GameBackend.Application.Contracts.Sessions;

namespace GameBackend.Application.Validators.Sessions;

public class EndSessionRequestValidator : AbstractValidator<EndSessionRequest>
{
    public EndSessionRequestValidator()
    {
        RuleFor(x => x.Score)
            .GreaterThanOrEqualTo(0).WithMessage("Score must be a positive number");
    }
}