using SQS_ServiceModel;

namespace SQS_ServiceLib.Handler
{
    public class FileType2Handler : IFileHandler
    {
        public static FileType Type => FileType.FILE_TYPE_2;

        public Task HandleAsync(string inputXml)
        {
            Console.WriteLine($"File Type 2 {inputXml}");
            return Task.CompletedTask;
        }
    }
}
