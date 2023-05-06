using Microsoft.AspNetCore.Mvc;

namespace SQS_ServiceJob.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DefaultController : ControllerBase
    {
        private readonly ILogger<DefaultController> _logger;

        public DefaultController(ILogger<DefaultController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            _logger.LogDebug("Service Started...");
            return await Task.FromResult("Service Running...");
        }
    }
}