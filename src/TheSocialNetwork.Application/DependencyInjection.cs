using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TheSocialNetwork.Application.UserProfiles.ChangeUserProfileVisibility;
using TheSocialNetwork.Application.UserProfiles.ChangeUsername;
using TheSocialNetwork.Application.UserProfiles.CreateUserProfile;
using TheSocialNetwork.Application.UserProfiles.UpdateUserProfile;

namespace TheSocialNetwork.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateUserProfileCommand>, CreateUserProfileCommandValidator>();
        services.AddScoped<CreateUserProfileCommandHandler>();

        services.AddScoped<IValidator<ChangeUsernameCommand>, ChangeUsernameCommandValidator>();
        services.AddScoped<ChangeUsernameCommandHandler>();

        services.AddScoped<IValidator<ChangeUserProfileVisibilityCommand>, ChangeUserProfileVisibilityCommandValidator>();
        services.AddScoped<ChangeUserProfileVisibilityCommandHandler>();

        services.AddScoped<IValidator<UpdateUserProfileCommand>, UpdateUserProfileCommandValidator>();
        services.AddScoped<UpdateUserProfileCommandHandler>();

        return services;
    }
}
