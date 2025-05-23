using TimeTracker.Domain.Options.Base;

namespace TimeTracker.Domain.Options;

public class AuthOptions : IOption
{
    public static string Section { get; } = "AuthOptions";

    public required string SecurityKey { get; set; }
    public int ExpirationHours { get; set; }

    public required string UserKey { get; set; }
    public required string Authority { get; set; }
}
