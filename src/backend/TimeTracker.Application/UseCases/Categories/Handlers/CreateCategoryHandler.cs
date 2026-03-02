using LiteBus.Commands.Abstractions;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Domain;

namespace TimeTracker.Application.UseCases.Categories.Handlers;

public class CreateCategoryHandler : ICommandHandler<CreateCategoryHandler.Command, Guid>
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Guid> HandleAsync(Command request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            OrganizationId = request.OrganizationId
        };

        await _categoryRepository.InsertAsync(category, true, cancellationToken);

        return category.Id;
    }

    public record Command(string Name, string? Description, Guid? OrganizationId) : ICommand<Guid>;
}