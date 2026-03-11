using Bogus;
using TimeTracker.Application.UseCases.Categories.Handlers;

namespace TimeTracker.UnitTests.Common.Builders.Application.Categories;

public class UpdateCategoryCommandBuilder : EntityBuilder<UpdateCategoryCommandBuilder, UpdateCategoryHandler.Command>
{
    private static readonly Faker Faker = new();

    private Guid _id = Guid.NewGuid();
    private string _name = Faker.Commerce.Categories(1).First();
    private string? _description = Faker.Lorem.Sentence();

    protected override UpdateCategoryHandler.Command Instance => new(_id, _name, _description);

    public UpdateCategoryCommandBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public UpdateCategoryCommandBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public UpdateCategoryCommandBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    public UpdateCategoryCommandBuilder WithoutDescription()
    {
        _description = null;
        return this;
    }
}
