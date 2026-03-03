using FluentValidation;
using LiteBus.Commands.Abstractions;
using LiteBus.Messaging.Abstractions;
using LiteBus.Queries.Abstractions;

namespace TimeTracker.Application.Behaviours;

public class ValidationPreHandler<TRequest> : IAsyncMessagePreHandler<TRequest> where TRequest : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPreHandler(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }
    public async Task PreHandleAsync(TRequest request, CancellationToken cancellationToken)
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