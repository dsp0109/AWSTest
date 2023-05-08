using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.SQS;
using Microsoft.AspNetCore.Mvc.Filters;
using SQS_ServiceJob.Filters;
using SQS_ServiceJob.Jobs;
using SQS_ServiceLib.BusinessLogic;
using SQS_ServiceLib.Handler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(option =>
{
    option.Filters.Add<UnhandledExceptionFilterAttribute>();
});

builder.Services.AddHostedService<OutboundMessageWatcherJob>();

builder.Services.AddSingleton<IAmazonSQS>(x => {
    var credentials = new BasicAWSCredentials(Convert.ToString(builder.Configuration.GetValue(typeof(string), "AWSCred:AccessKey")), Convert.ToString(builder.Configuration.GetValue(typeof(string), "AWSCred:Secret")));
    return new AmazonSQSClient(credentials, RegionEndpoint.GetBySystemName(Convert.ToString(builder.Configuration.GetValue(typeof(string), "AWSCred:OutboundSQS:Region"))));
});
builder.Services.AddSingleton<IAmazonS3>(x =>
{
    var credentials = new BasicAWSCredentials(Convert.ToString(builder.Configuration.GetValue(typeof(string), "AWSCred:AccessKey")), Convert.ToString(builder.Configuration.GetValue(typeof(string), "AWSCred:Secret")));
    return new AmazonS3Client(credentials, RegionEndpoint.GetBySystemName(Convert.ToString(builder.Configuration.GetValue(typeof(string), "AWSCred:S3Bucket:Region"))));
});
builder.Services.AddTransient<IProcessFile, ProcessFile>();
builder.Services.AddScoped<FileType1Handler>();
builder.Services.AddScoped<FileType2Handler>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
