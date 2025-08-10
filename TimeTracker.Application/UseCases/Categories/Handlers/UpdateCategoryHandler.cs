using MediatR;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Domain;

namespace TimeTracker.Application.UseCases.Categories.Handlers;

public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryHandler.Command, bool>
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategoryHandler(ICategoryRepository categoryRepository)
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

        existingCategory.Name = request.Name;
        existingCategory.Description = request.Description;

        var result = await _categoryRepository.UpdateAsync(existingCategory, true, cancellationToken);
        
        return result > 0;
    }

    public record Command(Guid Id, string Name, string? Description) : IRequest<bool>;
}