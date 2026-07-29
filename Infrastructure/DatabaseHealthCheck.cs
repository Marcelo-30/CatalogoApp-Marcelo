using CatalogoRopaMVC.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CatalogoRopaMVC.Infrastructure;

public sealed class DatabaseHealthCheck : IHealthCheck
{
    private readonly ApplicationDbContext _dbContext;

    public DatabaseHealthCheck(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);

            return canConnect
                ? HealthCheckResult.Healthy("Database connection is available.")
                : HealthCheckResult.Unhealthy("Database connection is unavailable.");
        }
        catch
        {
            return HealthCheckResult.Unhealthy("Database connection is unavailable.");
        }
    }
}
