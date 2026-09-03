using FluentValidation;
using TheSocialNetwork.Domain.UserProfiles;

namespace TheSocialNetwork.Application.UserProfiles.CreateUserProfile;

public sealed class CreateUserProfileCommandValidator : AbstractValidator<CreateUserProfileCommand>
{
    public CreateUserProfileCommandValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty()
            .WithMessage(UserProfileErrors.InvalidUserId.Description);

        RuleFor(c => c.Username)
            .Must(value => Username.Create(value).IsSuccess)
            .WithMessage(UserProfileErrors.InvalidUsername.Description);

        RuleFor(c => c.DisplayName)
            .Must(value => DisplayName.Create(value).IsSuccess)
            .WithMessage(UserProfileErrors.InvalidDisplayName.Description);
    }
}
