using MediatR;
using TimeTracker.Application.Interfaces.Data;

namespace TimeTracker.Application.UseCases.Categories.Handlers;

public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryHandler.Command>
{
    private readonly ICategoryRepository _categoryRepository;

    public DeleteCategoryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        await _categoryRepository.DeleteAsync(request.Id, cancellationToken);
    }

    public record Command(Guid Id) : IRequest;
}