using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RandomFact_Slack.Core;
using RandomFact_Slack.Core.Authentication;
using RandomFact_Slack.Core.Dto;
using RandomFact_Slack.Core.Services;

namespace RandomFact_Slack.Api.Controllers
{
    [Route("{tenant}/[controller]/")]
    [RequestAuth]
    [ApiController]
    public class FactController : ControllerBase
    {
        private readonly IFactService _factService;

        public FactController(IFactService factService)
        {
            _factService = factService;
        }

        [HttpPost]
        public async Task<IActionResult> GetFact(string tenant)
        {
            var result = await _factService.GetFact(tenant);
            return Ok(new SlackBasicResponseResponseDto
            {
                response_type = BasicResponseTypes.in_channel,
                text = result
            });
        }
    }
}