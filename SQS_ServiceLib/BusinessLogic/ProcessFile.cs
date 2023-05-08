using Amazon.S3;
using Amazon.S3.Util;
using Microsoft.Extensions.DependencyInjection;
using SQS_ServiceLib.Handler;
using SQS_ServiceModel;
using System.Reflection;

namespace SQS_ServiceLib.BusinessLogic
{
    public class ProcessFile : IProcessFile
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IAmazonS3 _s3Client;
        private readonly Dictionary<FileType, Func<IServiceProvider, IFileHandler>> _handlers;
        private readonly Dictionary<string, FileType> _fileNameTypeMapping;

        public ProcessFile(IAmazonS3 s3Client, IServiceScopeFactory serviceScopeFactory)
        {
            _s3Client = s3Client;
            _serviceScopeFactory = serviceScopeFactory;
            _handlers = Assembly.GetExecutingAssembly().DefinedTypes!
                .Where(hdlr => typeof(IFileHandler).IsAssignableFrom(hdlr) && !hdlr.IsInterface && !hdlr.IsAbstract)!
                .ToDictionary<TypeInfo, FileType, Func<IServiceProvider, IFileHandler>>(
                    info => (FileType)info.GetProperty(nameof(IFileHandler.Type))!.GetValue(info)!,
                    info => provider => (IFileHandler)provider.GetRequiredService(info.AsType())
                );
            _fileNameTypeMapping = new Dictionary<string, FileType>() 
            {
                { "FILE1", FileType.FILE_TYPE_1 },
                { "FILE2", FileType.FILE_TYPE_2 },
                { "FILE3", FileType.FILE_TYPE_3 },
                { "FILE4", FileType.FILE_TYPE_4 },
            };
        }

        public async Task<FileDetails?> GetFileData(string bucketName, string fileName, CancellationToken cancellationToken)
        {
            var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
            if (!bucketExists) throw new FileNotFoundException($"File not found in bucket {bucketName} with name {fileName}");
            var s3Object = await _s3Client.GetObjectAsync(bucketName, fileName);
            
            if(s3Object.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new FileNotFoundException($"File not found in bucket {bucketName} with name {fileName}");
            }
            string tempFileName = Path.GetTempPath() + Guid.NewGuid().ToString() + ".file";
            await s3Object.WriteResponseStreamToFileAsync(tempFileName, true, cancellationToken);
            var fileType = GetFileTypeFromName(fileName);

            if(fileName == null)
            {
                // Unhandled file type
                return null;
            }

            var resultData = new FileDetails { FileContent = File.ReadAllText(tempFileName), FileName = fileName, Type = fileType!.Value };
            File.Delete(tempFileName);

            return await Task.FromResult(resultData);
        }

        public async Task<bool> ProcessFileData(FileDetails fileDetails)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var fileHandler = _handlers[fileDetails.Type](scope.ServiceProvider);
            await fileHandler.HandleAsync(fileDetails.FileContent);
            return await Task.FromResult<bool>(true);
        }

        private FileType? GetFileTypeFromName(string fileName)
        {
            return _fileNameTypeMapping.Where(x => fileName.StartsWith(x.Key))?.FirstOrDefault().Value;
        }

    }
}
