using TimeTracker.Domain.Auth;

namespace TimeTracker.Application.Interfaces.Auth;

public interface ITokenProvider
{
    string CreateAuthToken(User user);

    string? CreateRememberMeToken(string email);
    string? TryGetLoginEmailFromRememberMeToken(string? rememberMeToken);
}
