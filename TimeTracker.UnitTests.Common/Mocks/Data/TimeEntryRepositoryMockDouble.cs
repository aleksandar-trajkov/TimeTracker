using NSubstitute;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Domain;

namespace TimeTracker.UnitTests.Common.Mocks.Data;

public class TimeEntryRepositoryMockDouble : MockDouble<ITimeEntryRepository>
{
    public void GivenGetByIdAsync(Guid id, TimeEntry timeEntry)
    {
        Instance.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(timeEntry);
    }

    public void GivenFindByIdAsync(Guid id, TimeEntry? timeEntry)
    {
        Instance.FindByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(timeEntry);
    }

    public void GivenExistsAsync(Guid id, bool exists)
    {
        Instance.ExistsAsync(id, Arg.Any<CancellationToken>())
            .Returns(exists);
    }

    public void GivenGetAllAsync(Guid userId, DateTimeOffset from, DateTimeOffset to, List<TimeEntry> timeEntries)
    {
        Instance.GetAllAsync(userId, from, to, Arg.Any<CancellationToken>())
            .Returns(timeEntries);
    }

    public void GivenInsertAsync()
    {
        Instance.InsertAsync(Arg.Any<TimeEntry>(), true, Arg.Any<CancellationToken>())
            .Returns(1);
    }

    public void GivenUpdateAsync()
    {
        Instance.UpdateAsync(Arg.Any<TimeEntry>(), true, Arg.Any<CancellationToken>())
            .Returns(1);
    }

    public void GivenDeleteAsync()
    {
        Instance.DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
    }
}
