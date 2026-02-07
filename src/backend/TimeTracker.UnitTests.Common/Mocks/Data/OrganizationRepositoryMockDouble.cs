using NSubstitute;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Domain;

namespace TimeTracker.UnitTests.Common.Mocks.Data;

public class OrganizationRepositoryMockDouble : MockDouble<IOrganizationRepository>
{
    public void GivenExistsAsync(Guid organizationId, bool exists)
    {
        Instance.ExistsAsync(organizationId, Arg.Any<CancellationToken>())
            .Returns(exists);
    }

    public void GivenGetByIdAsync(Guid organizationId, Organization organization)
    {
        Instance.GetByIdAsync(organizationId, Arg.Any<CancellationToken>())
            .Returns(organization);
    }

    public void GivenFindByIdAsync(Guid organizationId, Organization? organization)
    {
        Instance.FindByIdAsync(organizationId, Arg.Any<CancellationToken>())
            .Returns(organization);
    }
}