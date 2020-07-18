using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RandomFact_Slack.Infrastructure.Service;

namespace RandomFact_Slack.Core.Services
{
    public interface IFactService
    {
        Task<string> GetFact();
    }

    public class FactService : IFactService
    {
        private readonly IRandomFactApiService _randomFactApiService;
        private readonly ILogger<FactService> _logger;

        public FactService(IRandomFactApiService randomFactApiService, ILogger<FactService> logger)
        {
            _randomFactApiService = randomFactApiService;
            _logger = logger;
        }

        public async Task<string> GetFact()
        {
            var fact = await _randomFactApiService.GetFact();

            if (fact == null || string.IsNullOrEmpty(fact.Text))
            {
                _logger.LogError("Fact result was empty");
                throw new Exception("Oh no there was no fact");
            }

            return fact.Text;
        }
    }
}