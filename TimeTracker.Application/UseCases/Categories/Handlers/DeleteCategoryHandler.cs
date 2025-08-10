using MediatR;
using TimeTracker.Application.Interfaces.Data;

namespace TimeTracker.Application.UseCases.Categories.Handlers;

public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryHandler.Command, bool>
{
    private readonly ICategoryRepository _categoryRepository;

    public DeleteCategoryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
    {
        var result = await _categoryRepository.DeleteAsync(request.Id, true, cancellationToken);
        
        return result > 0;
    }

    public record Command(Guid Id) : IRequest<bool>;
}