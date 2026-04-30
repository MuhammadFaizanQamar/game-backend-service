using FluentValidation;
using GameBackend.Application.Contracts.Leaderboards;

namespace GameBackend.Application.Validators.Leaderboards;

public class SubmitScoreRequestValidator : AbstractValidator<SubmitScoreRequest>
{
    public SubmitScoreRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Leaderboard name is required")
            .MaximumLength(50).WithMessage("Leaderboard name must not exceed 50 characters");

        RuleFor(x => x.ScoreType)
            .NotEmpty().WithMessage("Score type is required")
            .MaximumLength(20).WithMessage("Score type must not exceed 20 characters");

        RuleFor(x => x.Score)
            .GreaterThanOrEqualTo(0).WithMessage("Score must be a positive number");
    }
}