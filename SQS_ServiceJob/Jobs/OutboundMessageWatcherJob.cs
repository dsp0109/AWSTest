using Amazon.SQS;
using Amazon.SQS.Model;
using SQS_ServiceLib.BusinessLogic;
using SQS_ServiceModel;
using System.Text;
using System.Text.RegularExpressions;

namespace SQS_ServiceJob.Jobs
{
    public class OutboundMessageWatcherJob : BackgroundService
    {
        private readonly IAmazonSQS _sqs;
        private readonly IConfiguration _config;
        private readonly ILogger<OutboundMessageWatcherJob> _logger;
        private readonly string? _queueName;
        private readonly string? _outboundS3BucketName;
        private readonly string? _inboundS3BucketName;
        private readonly IProcessFile _processFile;

        private readonly Dictionary<FileType, string> handlerIdentifier;

        public OutboundMessageWatcherJob(IAmazonSQS sqs, IConfiguration config, ILogger<OutboundMessageWatcherJob> logger, IProcessFile processFile)
        {
            _sqs = sqs;
            _config = config;
            _logger = logger;
            _queueName = Convert.ToString(_config!.GetValue(typeof(string), "AWSCred:OutboundSQS:QueueName"));
            _outboundS3BucketName = Convert.ToString(_config!.GetValue(typeof(string), "AWSCred:S3Bucket:OutboundBucketName"));
            _inboundS3BucketName = Convert.ToString(_config!.GetValue(typeof(string), "AWSCred:S3Bucket:InboundBucketName"));
            _processFile = processFile;

            handlerIdentifier = new Dictionary<FileType, string>();
            handlerIdentifier.Add(FileType.FILE_TYPE_1, "^INBOUND.*\\.(xml|XML)$");
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var queryUrl = await _sqs.GetQueueUrlAsync(_queueName, stoppingToken);
                var receiveMessageRequest = new ReceiveMessageRequest
                {
                    QueueUrl = queryUrl.QueueUrl,
                };

                while (!stoppingToken.IsCancellationRequested)
                {
                    var messageResponse = await _sqs.ReceiveMessageAsync(receiveMessageRequest, stoppingToken);
                    if (messageResponse == null || messageResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
                    {
                        _logger.LogInformation($"{nameof(OutboundMessageWatcherJob)} Failed with not ok response.");
                        continue;
                    }

                    foreach (var message in messageResponse!.Messages)
                    {
                        var fileTypeeHandler = handlerIdentifier.Where(x => Regex.IsMatch(message.Body, x.Value)).FirstOrDefault();
                        if (handlerIdentifier.TryGetValue((FileType)Enum.Parse(typeof(FileType), message.Body), out string? regexPattern))
                        {
                            if (!Regex.IsMatch(message.Body, regexPattern))
                            {
                                _logger.LogInformation($"{nameof(OutboundMessageWatcherJob)} File is not supported for processing.");
                                continue;
                            }
                        }
                        else
                        {
                            _logger.LogInformation($"{nameof(OutboundMessageWatcherJob)} File is not supported for processing.");
                            continue;
                        }   

                        #region File Processing 
                        var fileDetails = await _processFile.GetFileData(_outboundS3BucketName!, message.Body, stoppingToken);
                        if (fileDetails == null)
                        {
                            _logger.LogInformation($"{nameof(OutboundMessageWatcherJob)} File is not supported for processing.");
                            continue;
                        }
                        var fileProcessingResult = await _processFile.ProcessFileData(fileDetails);
                        if (fileProcessingResult)
                        {
                            await _processFile.UploadFile(_inboundS3BucketName!, new MemoryStream(Encoding.UTF8.GetBytes(fileDetails!.FileContent)), fileDetails.FileName, stoppingToken);
                            await _sqs.DeleteMessageAsync(queryUrl.QueueUrl, message.ReceiptHandle, stoppingToken);
                        }
                        
                        #endregion 
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(OutboundMessageWatcherJob)} failed processing with error {ex.Message}", ex);
                await Task.FromException(ex);
            }
        }
    }
}
