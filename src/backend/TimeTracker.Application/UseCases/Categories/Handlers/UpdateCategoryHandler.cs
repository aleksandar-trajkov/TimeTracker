using LiteBus.Commands.Abstractions;
using TimeTracker.Application.Interfaces.Data;

namespace TimeTracker.Application.UseCases.Categories.Handlers;

public class UpdateCategoryHandler : ICommandHandler<UpdateCategoryHandler.Command, bool>
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategoryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<bool> HandleAsync(Command request, CancellationToken cancellationToken)
    {
        var existingCategory = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);

        existingCategory.Name = request.Name;
        existingCategory.Description = request.Description;

        var result = await _categoryRepository.UpdateAsync(existingCategory, true, cancellationToken);

        return result > 0;
    }

    public record Command(Guid Id, string Name, string? Description) : ICommand<bool>;
}