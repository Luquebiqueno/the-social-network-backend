using FluentValidation;

namespace TheSocialNetwork.Application.Users.DeactivateUser;

public sealed class DeactivateUserCommandValidator : AbstractValidator<DeactivateUserCommand>
{
    public DeactivateUserCommandValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty();
    }
}
