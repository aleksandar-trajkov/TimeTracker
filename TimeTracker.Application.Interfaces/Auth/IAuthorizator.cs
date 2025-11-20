
namespace TimeTracker.Application.Interfaces.Auth;

public interface IAuthorizator<TRequest> where TRequest : class
{
    Task AuthorizeAsync(TRequest request, CancellationToken cancellationToken);
}