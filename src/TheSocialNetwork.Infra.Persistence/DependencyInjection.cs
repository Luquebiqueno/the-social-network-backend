using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using TheSocialNetwork.Application.Abstractions.Data;
using TheSocialNetwork.Domain.UserProfiles;
using TheSocialNetwork.Infra.Persistence.Data;
using TheSocialNetwork.Infra.Persistence.Repositories.UserProfiles;

namespace TheSocialNetwork.Infra.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.ConnectionString),
                "The database connection string is required.")
            .Validate(options => options.MaxPoolSize > 0,
                "MaxPoolSize must be greater than zero.")
            .ValidateOnStart();

        services.AddSingleton(sp => sp.GetRequiredService<IOptions<DatabaseOptions>>().Value);

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            var connectionString = new NpgsqlConnectionStringBuilder(options.ConnectionString)
            {
                Pooling = true,
                MinPoolSize = options.MinPoolSize,
                MaxPoolSize = options.MaxPoolSize,
                Timeout = 15,
                CommandTimeout = options.CommandTimeoutSeconds,
                KeepAlive = 30,
                ApplicationName = "the-social-network-api",
                IncludeErrorDetail = false
            };

            return new NpgsqlDataSourceBuilder(connectionString.ConnectionString).Build();
        });

        services.AddScoped<IUnitOfWork, DapperUnitOfWork>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        return services;
    }
}
