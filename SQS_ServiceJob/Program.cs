using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.SQS;
using Hangfire;
using Serilog;
using SQS_ServiceJob.Filters;
using SQS_ServiceJob.Health;
using SQS_ServiceJob.Jobs;
using SQS_ServiceLib.BusinessLogic;
using SQS_ServiceLib.Handler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(option =>
{
    option.Filters.Add<UnhandledExceptionFilterAttribute>();
});

// Serilogger
var logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).Enrich.FromLogContext().CreateLogger();
//builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

//builder.Services.AddHostedService<OutboundMessageWatcherJob>();
//builder.Services.AddHostedService<MasterdataSchedulerJobWithHF>();
builder.Services.AddHostedService<MasterdataSchedulerJobNormal>();

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

builder.Services.AddHangfire(confg => confg.UseInMemoryStorage()
                                           .UseRecommendedSerializerSettings());
builder.Services.AddHangfireServer();
builder.Services.AddTransient<IMasterdataProcessor, MasterdataProcessor>();
builder.Services.AddScoped<CatelogFileHandler>();
builder.Services.AddScoped<CatelogFileHandler1>();

builder.Services.AddHealthChecks().AddCheck<MonitorHealth>("MonitorHealth");

var app = builder.Build();

app.UseHangfireDashboard();
app.MapHangfireDashboard();
app.MapHealthChecks("/healthz");

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
