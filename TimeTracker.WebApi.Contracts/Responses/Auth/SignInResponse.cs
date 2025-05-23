namespace TimeTracker.WebApi.Contracts.Responses.Auth;

public class SignInResponse
{
    public string Token { get; init; } = string.Empty;
    public string? RememberMeToken { get; init; } = null;
}
