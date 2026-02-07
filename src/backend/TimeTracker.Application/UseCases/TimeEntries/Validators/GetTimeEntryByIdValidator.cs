using FluentValidation;
using TimeTracker.Application.Interfaces.Auth;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Application.UseCases.TimeEntries.Handlers;

namespace TimeTracker.Application.UseCases.TimeEntries.Validators;

public class GetTimeEntryByIdValidator : AbstractValidator<GetTimeEntryByIdHandler.Query>
{
    public GetTimeEntryByIdValidator(ITimeEntryRepository timeEntryRepository, IUserContext userContext)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Time entry ID is required.")
            .MustAsync(async (id, cancellation) => await timeEntryRepository.ExistsAsync(id, cancellation))
            .WithMessage("Time entry does not exist.")
            .MustAsync(async (query, id, cancellation) =>
            {
                var timeEntry = await timeEntryRepository.FindByIdAsync(id, cancellation);
                return timeEntry?.UserId == userContext.UserId;
            })
            .WithMessage("You can only access your own time entries.");
    }
}