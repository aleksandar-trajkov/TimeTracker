namespace TimeTracker.WebApi.Contracts.Requests.Auth;

public record SignInRequest
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public bool RememberMe { get; init; } = false;
}