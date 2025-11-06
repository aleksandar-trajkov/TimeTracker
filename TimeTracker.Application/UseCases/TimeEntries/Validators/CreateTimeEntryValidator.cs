using FluentValidation;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Application.UseCases.TimeEntries.Handlers;

namespace TimeTracker.Application.UseCases.TimeEntries.Validators;

public class CreateTimeEntryValidator : AbstractValidator<CreateTimeEntryHandler.Command>
{
    public CreateTimeEntryValidator(ICategoryRepository categoryRepository)
    {
        RuleFor(x => x.From)
            .NotEmpty()
            .WithMessage("Start time is required.")
            .LessThan(x => x.To)
            .WithMessage("Start time must be earlier than end time.");

        RuleFor(x => x.To)
            .NotEmpty()
            .WithMessage("End time is required.")
            .GreaterThan(x => x.From)
            .WithMessage("End time must be later than start time.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Category is required.")
            .MustAsync(async (categoryId, cancellation) => await categoryRepository.ExistsAsync(categoryId, cancellation))
            .WithMessage("Category does not exist.");

        RuleFor(x => x)
            .Must(x => x.To > x.From)
            .WithMessage("End time must be after start time.")
            .Must(x => (x.To - x.From).TotalHours <= 24)
            .WithMessage("Time entry cannot exceed 24 hours.");
    }
}