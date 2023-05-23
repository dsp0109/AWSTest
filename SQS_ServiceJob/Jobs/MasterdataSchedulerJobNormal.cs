using Cronos;
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
            //await RunTasks();
            try
            {
                await _masterdataProcessor.RegisterSchedulerNormal(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Log exception
            }
        }
    }
}
