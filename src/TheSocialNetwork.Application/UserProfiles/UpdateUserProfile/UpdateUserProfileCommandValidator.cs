using FluentValidation;
using TheSocialNetwork.Domain.UserProfiles;

namespace TheSocialNetwork.Application.UserProfiles.UpdateUserProfile;

public sealed class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(c => c.UserProfileId)
            .NotEmpty()
            .WithMessage(UserProfileErrors.InvalidUserProfileId.Description);

        RuleFor(c => c.DisplayName)
            .Must(value => DisplayName.Create(value).IsSuccess)
            .WithMessage(UserProfileErrors.InvalidDisplayName.Description);

        RuleFor(c => c.Biography)
            .Must(value => Biography.Create(value).IsSuccess)
            .WithMessage(UserProfileErrors.BiographyTooLong.Description);

        RuleFor(c => c.AvatarUrl)
            .Must(value => AvatarUrl.Create(value).IsSuccess)
            .WithMessage(UserProfileErrors.InvalidAvatarUrl.Description);
    }
}
