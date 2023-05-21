using Microsoft.Extensions.DependencyInjection;
using SQS_ServiceLib.BusinessLogic;

namespace SQS_ServiceJob.Jobs
{
    public class MasterdataSchedulerJobNormal : BackgroundService
    {
        private int _executionCount;
        private readonly IMasterdataProcessor _masterdataProcessor;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public MasterdataSchedulerJobNormal(IMasterdataProcessor masterdataProcessor, IServiceScopeFactory serviceScopeFactory)
        {
            _masterdataProcessor = masterdataProcessor;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await DoWork();
            using PeriodicTimer timer = new(TimeSpan.FromMinutes(2));
            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await DoWork();
                }
            }
            catch (OperationCanceledException)
            {
                // Log exception
            }
        }

        private async Task DoWork()
        {
            int count = Interlocked.Increment(ref _executionCount);
            // Log Counter of execution
            var jobToExecute = await _masterdataProcessor.GetJobs();
            Parallel.ForEach(jobToExecute, (job) => {
                using var scope = _serviceScopeFactory.CreateScope();
                job((IServiceProvider)scope).HandleAsync();
            });
            await Task.CompletedTask;
        }
    }
}
