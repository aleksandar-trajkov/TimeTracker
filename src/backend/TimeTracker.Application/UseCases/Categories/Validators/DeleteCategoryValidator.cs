using FluentValidation;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Application.UseCases.Categories.Handlers;

namespace TimeTracker.Application.UseCases.Categories.Validators;

public class DeleteCategoryValidator : AbstractValidator<DeleteCategoryHandler.Command>
{
    public DeleteCategoryValidator(ICategoryRepository categoryRepository)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Category ID is required.")
            .MustAsync(async (id, cancellation) => await categoryRepository.ExistsAsync(id, cancellation))
            .WithMessage("Category does not exist.");
    }
}