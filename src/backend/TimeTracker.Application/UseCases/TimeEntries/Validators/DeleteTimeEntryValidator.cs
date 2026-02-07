using FluentValidation;
using TimeTracker.Application.Interfaces.Auth;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Application.UseCases.TimeEntries.Handlers;

namespace TimeTracker.Application.UseCases.TimeEntries.Validators;

public class DeleteTimeEntryValidator : AbstractValidator<DeleteTimeEntryHandler.Command>
{
    public DeleteTimeEntryValidator(ITimeEntryRepository timeEntryRepository, IUserContext userContext)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Time entry ID is required.")
            .MustAsync(async (id, cancellation) => await timeEntryRepository.ExistsAsync(id, cancellation))
            .WithMessage("Time entry does not exist.")
            .MustAsync(async (command, id, cancellation) =>
            {
                var timeEntry = await timeEntryRepository.FindByIdAsync(id, cancellation);
                return timeEntry?.UserId == userContext.UserId;
            })
            .WithMessage("You can only delete your own time entries.");
    }
}