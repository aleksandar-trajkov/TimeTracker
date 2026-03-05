using MediatR;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Application.UseCases.Caching.Handlers;
using TimeTracker.Common.Caching;
using TimeTracker.Domain;

namespace TimeTracker.Application.UseCases.Categories.Handlers;

public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryHandler.Command, Category>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMediator _mediator;

    public UpdateCategoryHandler(ICategoryRepository categoryRepository, IMediator mediator)
    {
        _categoryRepository = categoryRepository;
        _mediator = mediator;
    }

    public async Task<Category> Handle(Command request, CancellationToken cancellationToken)
    {
        var existingCategory = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);

        existingCategory.Name = request.Name;
        existingCategory.Description = request.Description;

        var result = await _categoryRepository.UpdateAsync(existingCategory, true, cancellationToken);

        await _mediator.Publish(new ClearCacheHandler.Command(CachingKeys.Categories), cancellationToken);

        return existingCategory;
    }

    public record Command(Guid Id, string Name, string? Description) : IRequest<Category>;
}