using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RandomFact_Slack.Core.Authentication;
using RandomFact_Slack.Core.Services;

namespace RandomFact_Slack.Api.Controllers
{
    [Route("{tenant}/[controller]/")]
    [ApiKeyAuth]
    [ApiController]
    public class FactController : ControllerBase
    {
        private readonly IFactService _factService;

        public FactController(IFactService factService)
        {
            _factService = factService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFact()
        {

            var result = await _factService.GetFact();
            return Ok(result);
        }
    }
}