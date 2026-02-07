namespace TimeTracker.Application.UseCases.Auth.Dtos;

public class SignInResponseDto
{
    public required string Token { get; set; }
    public string? RememberMeToken { get; set; }
}