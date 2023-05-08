using SQS_ServiceModel;

namespace SQS_ServiceLib.Handler
{
    public interface IFileHandler
    {
        Task HandleAsync(string inputXml);

        public static FileType Type => throw new NotImplementedException();
    }
}
