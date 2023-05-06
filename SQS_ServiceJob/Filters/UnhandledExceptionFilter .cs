using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace SQS_ServiceJob.Filters
{
    public class UnhandledExceptionFilterAttribute : IExceptionFilter
    {
        private readonly ILogger<UnhandledExceptionFilterAttribute> _logger;

        public UnhandledExceptionFilterAttribute(ILogger<UnhandledExceptionFilterAttribute> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var result = new ObjectResult(new
            {
                context.Exception.Message,
                context.Exception.Source,
                ExceptionType = context.Exception.GetType().FullName,
            })
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };

            // Log the exception
            _logger.LogError($"Unhandled exception occurred while executing request: {context.Exception.Message}", context.Exception);

            // Set the result
            context.Result = result;
        }
    }
}
