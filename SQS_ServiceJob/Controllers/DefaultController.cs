using Microsoft.AspNetCore.Mvc;
using SQS_ServiceLib.BusinessLogic;

namespace SQS_ServiceJob.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DefaultController : ControllerBase
    {
        private readonly ILogger<DefaultController> _logger;
        private readonly IMasterdataProcessor _masterdataProcessor;
        public DefaultController(ILogger<DefaultController> logger, IMasterdataProcessor masterdataProcessor)
        {
            _logger = logger;
            _masterdataProcessor = masterdataProcessor;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            _logger.LogInformation("Service Started...");
            return await Task.FromResult("Service Running...");
        }
    }
}