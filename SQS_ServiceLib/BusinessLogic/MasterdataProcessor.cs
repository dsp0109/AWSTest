using SQS_ServiceLib.Handler;
using SQS_ServiceModel;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Hangfire;

namespace SQS_ServiceLib.BusinessLogic
{
    public class MasterdataProcessor : IMasterdataProcessor
    {
        private readonly List<MasterdataHandlerConfiguration>? _handlers;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public MasterdataProcessor(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _handlers = Assembly.GetExecutingAssembly().DefinedTypes?
                .Where(hdlr => typeof(IMasterdataFileHandler).IsAssignableFrom(hdlr) && !hdlr.IsInterface && !hdlr.IsAbstract)?
                .Select(
                    info => new MasterdataHandlerConfiguration
                    {
                        Type = (MasterDataType)info.GetProperty(nameof(IMasterdataFileHandler.Type))!.GetValue(info)!,
                        Crons = (List<string>)info.GetProperty(nameof(IMasterdataFileHandler.CronScheduler))!.GetValue(info)!,
                        JobId = (string)info.GetProperty(nameof(IMasterdataFileHandler.JobId))!.GetValue(info)!,
                        Handler = provider => (IMasterdataFileHandler)provider.GetRequiredService(info.AsType())
                    }
                )!.ToList();
        }

        public Task<IEnumerable<Func<IServiceProvider, IMasterdataFileHandler>>> GetJobs()
        {
            return Task.FromResult(_handlers!.Select(x => x.Handler));
        }

        public async Task<bool> RegisterScheduler()
        {
            if(_handlers == null )
            {
                return await Task.FromResult(false);
            }

            using var scope = _serviceScopeFactory.CreateScope();
            foreach (var handler in _handlers!)
            {
                var multipleIniatorNo = 0;
                foreach(var cron in handler.Crons)
                {
                    multipleIniatorNo++;
                    //RecurringJob.RemoveIfExists($"{handler.JobId}_{multipleIniatorNo}");
                    RecurringJob.AddOrUpdate($"{handler.JobId}_{multipleIniatorNo}", () => handler.Handler(scope.ServiceProvider).HandleAsync(), cron);
                }
            }

            return await Task.FromResult(true);
        }
    }
}
