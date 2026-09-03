using FluentValidation;
using TheSocialNetwork.Domain.UserProfiles;

namespace TheSocialNetwork.Application.UserProfiles.ChangeUsername;

public sealed class ChangeUsernameCommandValidator : AbstractValidator<ChangeUsernameCommand>
{
    public ChangeUsernameCommandValidator()
    {
        RuleFor(c => c.UserProfileId)
            .NotEmpty()
            .WithMessage(UserProfileErrors.InvalidUserProfileId.Description);

        RuleFor(c => c.NewUsername)
            .Must(value => Username.Create(value).IsSuccess)
            .WithMessage(UserProfileErrors.InvalidUsername.Description);
    }
}
