using SQS_ServiceModel;
using System.Collections;

namespace SQS_ServiceLib.Handler
{
    public interface IMasterdataFileHandler
    {
        public static MasterDataType Type => throw new NotImplementedException();

        public static List<string> CronScheduler => throw new NotImplementedException();

        public static List<long> ScheduleInMinutes => throw new NotImplementedException();

        public List<string> RunWithCrons => throw new NotImplementedException();

        public static string JobId => throw new NotImplementedException();

        Task HandleAsync();
    }
}
