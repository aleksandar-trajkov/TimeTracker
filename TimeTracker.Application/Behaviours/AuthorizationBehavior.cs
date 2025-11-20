using FluentValidation;
using MediatR;
using TimeTracker.Application.Interfaces.Auth;

namespace TimeTracker.Application.Behaviours;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    private readonly IEnumerable<IAuthorizator<TRequest>> _authorizators;
    public AuthorizationBehavior(IEnumerable<IAuthorizator<TRequest>> authorizators)
    {
        _authorizators = authorizators;
    }
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        foreach (var authorizator in _authorizators)
        {
            await authorizator.AuthorizeAsync(request, cancellationToken);
        }

        return await next().ConfigureAwait(false);
    }
}