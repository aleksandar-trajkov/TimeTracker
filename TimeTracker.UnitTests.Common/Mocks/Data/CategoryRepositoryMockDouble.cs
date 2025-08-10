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

    public void GivenInsertAsync()
    {
        Instance.InsertAsync(Arg.Any<Category>(), true, Arg.Any<CancellationToken>())
            .Returns(1);
    }

    public void GivenInsertAsync(CancellationToken cancellationToken)
    {
        Instance.InsertAsync(Arg.Any<Category>(), true, cancellationToken)
            .Returns(1);
    }

    public void GivenUpdateAsync()
    {
        Instance.UpdateAsync(Arg.Any<Category>(), true, Arg.Any<CancellationToken>())
            .Returns(1);
    }

    public void GivenUpdateAsync(CancellationToken cancellationToken)
    {
        Instance.UpdateAsync(Arg.Any<Category>(), true, cancellationToken)
            .Returns(1);
    }

    public void GivenDeleteAsync()
    {
        Instance.DeleteAsync(Arg.Any<Category>(), true, Arg.Any<CancellationToken>())
            .Returns(1);
    }

    public void GivenDeleteAsync(CancellationToken cancellationToken)
    {
        Instance.DeleteAsync(Arg.Any<Category>(), true, cancellationToken)
            .Returns(1);
    }

    public async Task VerifyInsertAsyncWasCalledWith(Category category, CancellationToken cancellationToken)
    {
        await Instance.Received(1).InsertAsync(
            Arg.Is<Category>(c => 
                c.Name == category.Name && 
                c.Description == category.Description && 
                c.OrganizationId == category.OrganizationId),
            true,
            cancellationToken);
    }

    public async Task VerifyUpdateAsyncWasCalledWith(Category category, CancellationToken cancellationToken)
    {
        await Instance.Received(1).UpdateAsync(
            Arg.Is<Category>(c => 
                c.Id == category.Id && 
                c.Name == category.Name && 
                c.Description == category.Description),
            true,
            cancellationToken);
    }

    public async Task VerifyDeleteAsyncWasCalledWith(Category category, CancellationToken cancellationToken)
    {
        await Instance.Received(1).DeleteAsync(
            Arg.Is<Category>(c => 
                c.Id == category.Id),
            true,
            cancellationToken);
    }
}