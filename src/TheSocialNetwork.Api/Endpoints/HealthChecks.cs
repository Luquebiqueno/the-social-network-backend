using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace TheSocialNetwork.Api.Endpoints;

internal sealed class HealthChecks : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false
        });

        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready")
        });
    }
}
