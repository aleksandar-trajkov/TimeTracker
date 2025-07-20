using FluentValidation;
using TimeTracker.Application.Interfaces.Auth;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Application.UseCases.Auth.Handlers;

namespace TimeTracker.Application.UseCases.Auth.Validators
{
    public class RememberMeSignInRequestHandlerValidator : AbstractValidator<RememberMeSignInRequestHandler.Query>
    {
        public RememberMeSignInRequestHandlerValidator(ITokenProvider tokenProvider, IUserRepository userRepository)
        {
            RuleFor(x => x.RememberMeToken)
                .NotEmpty()
                .WithMessage("Remember me token is required.");
        }
    }
}