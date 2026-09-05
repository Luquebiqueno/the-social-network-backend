using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TheSocialNetwork.Application.Abstractions.Messaging;
using TheSocialNetwork.Application.UserProfiles.ChangeUserProfileVisibility;
using TheSocialNetwork.Application.UserProfiles.ChangeUsername;
using TheSocialNetwork.Application.UserProfiles.CreateUserProfile;
using TheSocialNetwork.Application.UserProfiles.UpdateUserProfile;
using TheSocialNetwork.Application.Users.ChangeUserEmail;
using TheSocialNetwork.Application.Users.DeactivateUser;
using TheSocialNetwork.Application.Users.ReactivateUser;
using TheSocialNetwork.Application.Users.RegisterUser;
using TheSocialNetwork.Application.Users.SuspendUser;
using TheSocialNetwork.Application.Users.VerifyEmail;
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateUserProfileCommand>, CreateUserProfileCommandValidator>();
        services.AddScoped<ICommandHandler<CreateUserProfileCommand, Result<Guid>>, CreateUserProfileCommandHandler>();

        services.AddScoped<IValidator<ChangeUsernameCommand>, ChangeUsernameCommandValidator>();
        services.AddScoped<ICommandHandler<ChangeUsernameCommand, Result>, ChangeUsernameCommandHandler>();

        services.AddScoped<IValidator<ChangeUserProfileVisibilityCommand>, ChangeUserProfileVisibilityCommandValidator>();
        services.AddScoped<ICommandHandler<ChangeUserProfileVisibilityCommand, Result>, ChangeUserProfileVisibilityCommandHandler>();

        services.AddScoped<IValidator<UpdateUserProfileCommand>, UpdateUserProfileCommandValidator>();
        services.AddScoped<ICommandHandler<UpdateUserProfileCommand, Result>, UpdateUserProfileCommandHandler>();

        services.AddScoped<IValidator<RegisterUserCommand>, RegisterUserCommandValidator>();
        services.AddScoped<ICommandHandler<RegisterUserCommand, Result<Guid>>, RegisterUserCommandHandler>();

        services.AddScoped<IValidator<ChangeUserEmailCommand>, ChangeUserEmailCommandValidator>();
        services.AddScoped<ICommandHandler<ChangeUserEmailCommand, Result>, ChangeUserEmailCommandHandler>();

        services.AddScoped<IValidator<VerifyEmailCommand>, VerifyEmailCommandValidator>();
        services.AddScoped<ICommandHandler<VerifyEmailCommand, Result>, VerifyEmailCommandHandler>();

        services.AddScoped<IValidator<SuspendUserCommand>, SuspendUserCommandValidator>();
        services.AddScoped<ICommandHandler<SuspendUserCommand, Result>, SuspendUserCommandHandler>();

        services.AddScoped<IValidator<ReactivateUserCommand>, ReactivateUserCommandValidator>();
        services.AddScoped<ICommandHandler<ReactivateUserCommand, Result>, ReactivateUserCommandHandler>();

        services.AddScoped<IValidator<DeactivateUserCommand>, DeactivateUserCommandValidator>();
        services.AddScoped<ICommandHandler<DeactivateUserCommand, Result>, DeactivateUserCommandHandler>();

        return services;
    }
}
