using MediatR;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Application.UseCases.Caching.Handlers;
using TimeTracker.Common.Caching;

namespace TimeTracker.Application.UseCases.Categories.Handlers;

public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryHandler.Command>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMediator _mediator;

    public DeleteCategoryHandler(ICategoryRepository categoryRepository, IMediator mediator)
    {
        _categoryRepository = categoryRepository;
        _mediator = mediator;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        await _categoryRepository.DeleteAsync(request.Id, cancellationToken);

        await _mediator.Publish(new ClearCacheHandler.Command(CachingKeys.Categories), cancellationToken);
    }

    public record Command(Guid Id) : IRequest;
}