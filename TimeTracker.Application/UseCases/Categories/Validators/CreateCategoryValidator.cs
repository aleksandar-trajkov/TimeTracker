using FluentValidation;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Application.UseCases.Categories.Handlers;

namespace TimeTracker.Application.UseCases.Categories.Validators;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryHandler.Command>
{
    public CreateCategoryValidator(IOrganizationRepository organizationRepository)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Category name is required.")
            .MaximumLength(200)
            .WithMessage("Category name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Category description cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.OrganizationId)
            .MustAsync(async (orgId, cancellation) => await organizationRepository.ExistsAsync(orgId!.Value, cancellation))
            .WithMessage("Organization does not exist.")
            .When(x => x.OrganizationId.HasValue && x.OrganizationId != Guid.Empty);
    }
}