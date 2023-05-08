using SQS_ServiceModel;

namespace SQS_ServiceLib.Handler
{
    public class FileType1Handler : IFileHandler
    {
        public static FileType Type => FileType.FILE_TYPE_1;

        public Task HandleAsync(string inputXml)
        {
            Console.WriteLine($"File Type 1 {inputXml}");
            return Task.CompletedTask;
        }
    }
}
