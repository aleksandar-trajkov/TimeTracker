using FluentValidation;
using TimeTracker.Application.UseCases.TimeEntries.Handlers;

namespace TimeTracker.Application.UseCases.TimeEntries.Validators;

public class GetAllTimeEntriesValidator : AbstractValidator<GetAllTimeEntriesHandler.Query>
{
    public GetAllTimeEntriesValidator()
    {
        RuleFor(x => x.From)
            .NotEmpty()
            .WithMessage("Start date is required.")
            .LessThanOrEqualTo(x => x.To)
            .WithMessage("Start date must be earlier than or equal to end date.");

        RuleFor(x => x.To)
            .NotEmpty()
            .WithMessage("End date is required.")
            .GreaterThanOrEqualTo(x => x.From)
            .WithMessage("End date must be later than or equal to start date.");

        RuleFor(x => x)
            .Must(x => (x.To - x.From).TotalDays <= 365)
            .WithMessage("Date range cannot exceed 365 days.")
            .Must(x => x.From <= DateTimeOffset.Now.AddDays(1))
            .WithMessage("Start date cannot be more than 1 day in the future.")
            .Must(x => x.To <= DateTimeOffset.Now.AddDays(1))
            .WithMessage("End date cannot be more than 1 day in the future.");
    }
}