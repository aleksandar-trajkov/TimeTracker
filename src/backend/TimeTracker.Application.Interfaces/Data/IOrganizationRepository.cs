using TimeTracker.Application.Interfaces.Data.Base;
using TimeTracker.Domain;

namespace TimeTracker.Application.Interfaces.Data;

public interface IOrganizationRepository : IBaseRepository<Organization, Guid>
{
}