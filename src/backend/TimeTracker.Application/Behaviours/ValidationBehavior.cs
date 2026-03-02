using FluentValidation;
using LiteBus.Commands.Abstractions;
using LiteBus.Messaging.Abstractions;
using LiteBus.Queries.Abstractions;

namespace TimeTracker.Application.Behaviours;

public class CommandValidationBehavior<TRequest> : ValidationBehavior<TRequest>, ICommandPreHandler<TRequest> where TRequest : class, ICommand
{
    public CommandValidationBehavior(IEnumerable<IValidator<TRequest>> validators) : base(validators) { }

    public async Task PreHandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        await ValidateAsync(request, cancellationToken).ConfigureAwait(false);
    }
}

public class QueryValidationBehavior<TRequest> : ValidationBehavior<TRequest>, IQueryPreHandler<TRequest> where TRequest : class, IQuery
{
    public QueryValidationBehavior(IEnumerable<IValidator<TRequest>> validators) : base(validators) { }

    public async Task PreHandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        await ValidateAsync(request, cancellationToken).ConfigureAwait(false);
    }
}

public abstract class ValidationBehavior<TRequest> where TRequest : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }
    public async Task ValidateAsync(TRequest request, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken))).ConfigureAwait(false);
            var failures = validationResults
                .Where(r => r.Errors.Count > 0)
                .SelectMany(r => r.Errors)
                .ToList();
            if (failures.Count > 0)
                throw new ValidationException(failures);
        }
    }
}