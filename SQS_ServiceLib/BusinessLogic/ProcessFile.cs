using Amazon.S3;
using Amazon.S3.Util;
using Amazon.SQS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQS_ServiceLib.BusinessLogic
{
    public class ProcessFile : IProcessFile
    {
        private readonly IAmazonS3 _s3Client;

        public ProcessFile(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task<string> GetFileData(string bucketName, string fileName, CancellationToken cancellationToken)
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
            var resultData = File.ReadAllText(tempFileName);
            File.Delete(tempFileName);

            return await Task.FromResult(resultData);
        }

        public async Task<bool> ProcessFileData(string fileData)
        {
            Console.WriteLine(fileData);
            return await Task.FromResult<bool>(true);
        }
    }
}
