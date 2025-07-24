namespace TimeTracker.WebApi.Contracts.Responses.V1.Auth;

public class SignInResponse
{
    public string Token { get; init; } = string.Empty;
    public string? RememberMeToken { get; init; } = null;
}
