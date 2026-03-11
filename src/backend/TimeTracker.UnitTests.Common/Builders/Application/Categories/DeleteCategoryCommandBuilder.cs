using TimeTracker.Application.UseCases.Categories.Handlers;

namespace TimeTracker.UnitTests.Common.Builders.Application.Categories;

public class DeleteCategoryCommandBuilder : EntityBuilder<DeleteCategoryCommandBuilder, DeleteCategoryHandler.Command>
{
    private Guid _id = Guid.NewGuid();

    protected override DeleteCategoryHandler.Command Instance => new(_id);

    public DeleteCategoryCommandBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }
}
