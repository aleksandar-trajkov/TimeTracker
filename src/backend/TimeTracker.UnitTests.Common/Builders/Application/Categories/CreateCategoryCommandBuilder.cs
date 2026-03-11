using Bogus;
using TimeTracker.Application.UseCases.Categories.Handlers;

namespace TimeTracker.UnitTests.Common.Builders.Application.Categories;

public class CreateCategoryCommandBuilder : EntityBuilder<CreateCategoryCommandBuilder, CreateCategoryHandler.Command>
{
    private static readonly Faker Faker = new();

    private string _name = Faker.Commerce.Categories(1).First();
    private string? _description = Faker.Lorem.Sentence();
    private Guid? _organizationId = Guid.NewGuid();

    protected override CreateCategoryHandler.Command Instance => new(_name, _description, _organizationId);

    public CreateCategoryCommandBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public CreateCategoryCommandBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    public CreateCategoryCommandBuilder WithoutDescription()
    {
        _description = null;
        return this;
    }

    public CreateCategoryCommandBuilder WithOrganizationId(Guid? organizationId)
    {
        _organizationId = organizationId;
        return this;
    }
}
