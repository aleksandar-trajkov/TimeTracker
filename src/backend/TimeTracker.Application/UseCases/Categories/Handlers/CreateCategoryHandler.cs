using MediatR;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Application.UseCases.Caching.Handlers;
using TimeTracker.Common.Caching;
using TimeTracker.Domain;

namespace TimeTracker.Application.UseCases.Categories.Handlers;

public class CreateCategoryHandler : IRequestHandler<CreateCategoryHandler.Command, Category>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMediator _mediator;

    public CreateCategoryHandler(ICategoryRepository categoryRepository, IMediator mediator)
    {
        _categoryRepository = categoryRepository;
        _mediator = mediator;
    }

    public async Task<Category> Handle(Command request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            OrganizationId = request.OrganizationId
        };

        await _categoryRepository.InsertAsync(category, true, cancellationToken);

        await _mediator.Publish(new ClearCacheHandler.Command(CachingKeys.Categories), cancellationToken);

        return category;
    }

    public record Command(string Name, string? Description, Guid? OrganizationId) : IRequest<Category>;
}