using FluentValidation;
using TheSocialNetwork.Domain.UserProfiles;

namespace TheSocialNetwork.Application.UserProfiles.ChangeUserProfileVisibility;

public sealed class ChangeUserProfileVisibilityCommandValidator : AbstractValidator<ChangeUserProfileVisibilityCommand>
{
    public ChangeUserProfileVisibilityCommandValidator()
    {
        RuleFor(c => c.UserProfileId)
            .NotEmpty()
            .WithMessage(UserProfileErrors.InvalidUserProfileId.Description);

        RuleFor(c => c.Visibility)
            .IsInEnum();
    }
}
