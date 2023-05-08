using SQS_ServiceModel;

namespace SQS_ServiceLib.Handler
{
    public class FileType3Handler : IFileHandler
    {
        public static FileType Type => FileType.FILE_TYPE_3;

        public Task HandleAsync(string inputXml)
        {
            Console.WriteLine($"File Type 3 {inputXml}");
            return Task.CompletedTask;
        }
    }
}
