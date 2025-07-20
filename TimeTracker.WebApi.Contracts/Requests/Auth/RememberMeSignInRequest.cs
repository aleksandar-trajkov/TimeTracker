namespace TimeTracker.WebApi.Contracts.Requests.Auth;

public record RememberMeSignInRequest
{
    public string RememberMeToken { get; init; } = string.Empty;
}