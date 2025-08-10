using FluentValidation;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Application.UseCases.Categories.Handlers;

namespace TimeTracker.Application.UseCases.Categories.Validators;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryHandler.Command>
{
    public UpdateCategoryValidator(ICategoryRepository categoryRepository)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Category ID is required.")
            .MustAsync(async (id, cancellation) => await categoryRepository.ExistsAsync(id, cancellation))
            .WithMessage("Category does not exist.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Category name is required.")
            .MaximumLength(200)
            .WithMessage("Category name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Category description cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
    }
}