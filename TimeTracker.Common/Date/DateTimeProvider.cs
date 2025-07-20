using TimeTracker.Application.Interfaces.Common;

namespace TimeTracker.Common.Date;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
