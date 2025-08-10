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
        var existingCategory = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (existingCategory == null)
        {
            return false;
        }

        var result = await _categoryRepository.DeleteAsync(existingCategory, true, cancellationToken);
        
        return result > 0;
    }

    public record Command(Guid Id) : IRequest<bool>;
}