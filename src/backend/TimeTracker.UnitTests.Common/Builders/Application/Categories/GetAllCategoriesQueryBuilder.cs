using TimeTracker.Application.UseCases.Categories.Handlers;

namespace TimeTracker.UnitTests.Common.Builders.Application.Categories;

public class GetAllCategoriesQueryBuilder : EntityBuilder<GetAllCategoriesQueryBuilder, GetAllCategoriesHandler.Query>
{
    private Guid _organizationId = Guid.NewGuid();

    protected override GetAllCategoriesHandler.Query Instance => new(_organizationId);

    public GetAllCategoriesQueryBuilder WithOrganizationId(Guid organizationId)
    {
        _organizationId = organizationId;
        return this;
    }
}
