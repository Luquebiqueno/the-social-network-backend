using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TheSocialNetwork.Application.Abstractions.Messaging;
using TheSocialNetwork.Application.UserProfiles.ChangeUserProfileVisibility;
using TheSocialNetwork.Application.UserProfiles.ChangeUsername;
using TheSocialNetwork.Application.UserProfiles.CreateUserProfile;
using TheSocialNetwork.Application.UserProfiles.UpdateUserProfile;
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

        return services;
    }
}
