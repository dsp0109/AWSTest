using SQS_ServiceLib.Handler;
using SQS_ServiceModel;

namespace SQS_ServiceLib.BusinessLogic
{
    internal class MasterdataHandlerConfiguration
    {
        public MasterDataType Type { get; set; }

        public Func<IServiceProvider, IMasterdataFileHandler> Handler { get; set; }

        public List<string> Crons { get; set; }

        public string JobId { get; set; }
    }
}
