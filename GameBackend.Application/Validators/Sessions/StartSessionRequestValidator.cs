using FluentValidation;
using GameBackend.Application.Contracts.Sessions;

namespace GameBackend.Application.Validators.Sessions;

public class StartSessionRequestValidator : AbstractValidator<StartSessionRequest>
{
    public StartSessionRequestValidator()
    {
        RuleFor(x => x.GameId)
            .NotEmpty().WithMessage("GameId is required")
            .MaximumLength(50).WithMessage("GameId must not exceed 50 characters");
    }
}