using FluentValidation;
using TheSocialNetwork.Domain.Users;

namespace TheSocialNetwork.Application.Users.SuspendUser;

public sealed class SuspendUserCommandValidator : AbstractValidator<SuspendUserCommand>
{
    public SuspendUserCommandValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty();

        RuleFor(c => c.Reason)
            .Must(value => SuspensionReason.Create(value).IsSuccess)
            .WithMessage(UserErrors.InvalidSuspensionReason.Description);
    }
}
