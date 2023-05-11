using SQS_ServiceLib.BusinessLogic;

namespace SQS_ServiceJob.Jobs
{
    public class MasterdataSchedulerJobWithHF : BackgroundService
    {
        private readonly IMasterdataProcessor _masterdataProcessor;

        public MasterdataSchedulerJobWithHF(IMasterdataProcessor masterdataProcessor)
        {
            _masterdataProcessor = masterdataProcessor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var result = false;
            //while (!stoppingToken.IsCancellationRequested)
            //{
                result = await _masterdataProcessor.RegisterScheduler();
                //Thread.Sleep(300000);
            //}

            await Task.FromResult(result);
        }
    }
}
