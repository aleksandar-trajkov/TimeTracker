namespace TimeTracker.Domain.Exceptions;

public class AuthorizationException : Exception
{
    public AuthorizationException(string email, string permission)
    {
        Email = email;
        Permission = permission;
    }

    public AuthorizationException(string? message, string email, string permission) : base(message)
    {
        Email = email;
        Permission = permission;
    }

    public string Email { get; }
    public string Permission { get; }
}