namespace SQS_ServiceLib.BusinessLogic
{
    public interface IProcessFile
    {
        Task<bool> ProcessFileData(string fileData);

        Task<string> GetFileData(string bucketName, string fileName, CancellationToken cancellationToken);
    }
}
