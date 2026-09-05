using FluentValidation;
using TheSocialNetwork.Domain.Users;

namespace TheSocialNetwork.Application.Users.ChangeUserEmail;

public sealed class ChangeUserEmailCommandValidator : AbstractValidator<ChangeUserEmailCommand>
{
    public ChangeUserEmailCommandValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty();

        RuleFor(c => c.Email)
            .Must(value => Email.Create(value).IsSuccess)
            .WithMessage(UserErrors.InvalidEmail.Description);
    }
}
