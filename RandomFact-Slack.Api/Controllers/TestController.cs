using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace RandomFact_Slack.Api.Controllers
{
    [Route("{tenant}/Test")]
    public class TestController : Controller
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogError("Testing HELLO");
            return Ok();
        }
    }
}