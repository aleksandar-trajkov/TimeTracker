using MediatR;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Domain;

namespace TimeTracker.Application.UseCases.Categories.Handlers;

public class CreateCategoryHandler : IRequestHandler<CreateCategoryHandler.Command, Guid>
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
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

    public record Command(string Name, string? Description, Guid? OrganizationId) : IRequest<Guid>;
}