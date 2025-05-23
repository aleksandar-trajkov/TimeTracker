namespace TimeTracker.Domain.Exceptions
{
    public class AuthenticationException : Exception
    {
        public AuthenticationException(string email)
        {
            Email = email;
        }

        public AuthenticationException(string message, string email) : base(message)
        {
            Email = email;
        }

        public string Email { get; }
    }
}
