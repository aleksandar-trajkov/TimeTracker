using NSubstitute;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Domain;

namespace TimeTracker.UnitTests.Common.Mocks.Data;

public class CategoryRepositoryMockDouble : MockDouble<ICategoryRepository>
{
    public void GivenGetAllAsync(Guid organizationId, ICollection<Category> categories)
    {
        Instance.GetAllAsync(organizationId, Arg.Any<CancellationToken>())
            .Returns(categories);
    }

    public void GivenGetByIdAsync(Guid id, Category? category)
    {
        Instance.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(category);
    }

    public void GivenExistsAsync(Guid id, bool exists)
    {
        Instance.ExistsAsync(id, Arg.Any<CancellationToken>())
            .Returns(exists);
    }
}