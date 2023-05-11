using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SQS_ServiceJob.Health
{
    public class MonitorHealth : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var isHealthy = true;
            // Additional checks if any
            if (isHealthy)
            {
                return Task.FromResult(HealthCheckResult.Healthy("Service is healthy."));
            }

            return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, "Service is down."));
        }
    }
}
