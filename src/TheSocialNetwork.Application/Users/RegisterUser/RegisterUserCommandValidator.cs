using FluentValidation;
using TheSocialNetwork.Domain.Users;

namespace TheSocialNetwork.Application.Users.RegisterUser;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(c => c.Email)
            .Must(value => Email.Create(value).IsSuccess)
            .WithMessage(UserErrors.InvalidEmail.Description);

        RuleFor(c => c.ExternalIdentityProvider)
            .Must(value => ExternalIdentityProvider.Create(value).IsSuccess)
            .WithMessage(UserErrors.InvalidExternalIdentityProvider.Description);

        RuleFor(c => c.ExternalIdentitySubject)
            .Must(value => ExternalIdentitySubject.Create(value).IsSuccess)
            .WithMessage(UserErrors.InvalidExternalIdentitySubject.Description);
    }
}
