using FluentValidation;

namespace TheSocialNetwork.Application.Users.VerifyEmail;

public sealed class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailCommandValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty();
    }
}
