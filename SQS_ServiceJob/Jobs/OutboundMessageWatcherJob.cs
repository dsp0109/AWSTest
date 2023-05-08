using Amazon.SQS;
using Amazon.SQS.Model;
using SQS_ServiceLib.BusinessLogic;

namespace SQS_ServiceJob.Jobs
{
    public class OutboundMessageWatcherJob : BackgroundService
    {
        private readonly IAmazonSQS _sqs;
        private readonly IConfiguration _config;
        private readonly ILogger<OutboundMessageWatcherJob> _logger;
        private readonly string? _queueName;
        private readonly string? _outboundS3BucketName;
        private readonly IProcessFile _processFile;

        public OutboundMessageWatcherJob(IAmazonSQS sqs, IConfiguration config, ILogger<OutboundMessageWatcherJob> logger, IProcessFile processFile)
        {
            _sqs = sqs;
            _config = config;
            _logger = logger;
            _queueName = Convert.ToString(_config!.GetValue(typeof(string), "AWSCred:OutboundSQS:QueueName"));
            _outboundS3BucketName = Convert.ToString(_config!.GetValue(typeof(string), "AWSCred:OutboundS3Bucket:BucketName"));
            _processFile = processFile;
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
                        #region File Processing 
                        var fileDetails = await _processFile.GetFileData(_outboundS3BucketName!, message.Body, stoppingToken);
                        if (fileDetails == null)
                        {
                            _logger.LogInformation($"{nameof(OutboundMessageWatcherJob)} File is not supported for processing.");
                            continue;
                        }
                        var fileProcessingResult = await _processFile.ProcessFileData(fileDetails);
                        await _sqs.DeleteMessageAsync(queryUrl.QueueUrl, message.ReceiptHandle, stoppingToken);
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
