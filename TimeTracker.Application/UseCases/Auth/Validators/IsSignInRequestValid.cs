using FluentValidation;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Application.UseCases.Auth.Handlers;

namespace TimeTracker.Application.UseCases.Auth.Validators
{
    public class IsSignInRequestValid : AbstractValidator<SignIn.Query>
    {
        public IsSignInRequestValid(IUserRepository userRepository)
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.");

            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required");
            RuleFor(x => x.Password)
                .MinimumLength(8)
                .WithMessage("Password must be at least 6 characters long.");

            RuleFor(x => x.Email)
                .MustAsync(async (email, cancellation) => await userRepository.ExistsByEmailAsync(email, cancellation))
                .WithMessage("User with email does not exist in system");
        }
    }
}
