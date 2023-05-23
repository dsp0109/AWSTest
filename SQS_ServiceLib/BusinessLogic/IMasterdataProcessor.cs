using SQS_ServiceLib.Handler;

namespace SQS_ServiceLib.BusinessLogic
{
    public interface IMasterdataProcessor
    {
        Task<bool> RegisterScheduler();

        Task<IEnumerable<Func<IServiceProvider, IMasterdataFileHandler>>> GetJobs();

        Task<bool> RegisterSchedulerNormal(CancellationToken stoppingToken);
    }
}
