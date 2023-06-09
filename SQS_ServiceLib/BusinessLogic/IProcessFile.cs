﻿using SQS_ServiceModel;

namespace SQS_ServiceLib.BusinessLogic
{
    public interface IProcessFile
    {
        Task<bool> ProcessFileData(FileDetails fileDetails);

        Task<FileDetails?> GetFileData(string bucketName, string fileName, CancellationToken cancellationToken);

        Task<bool> UploadFile(string bucketName, Stream fileStream, string fileName, CancellationToken cancellationToken);
    }
}
