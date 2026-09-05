using FluentValidation;

namespace TheSocialNetwork.Application.Users.ReactivateUser;

public sealed class ReactivateUserCommandValidator : AbstractValidator<ReactivateUserCommand>
{
    public ReactivateUserCommandValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty();
    }
}
