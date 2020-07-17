using System.Threading.Tasks;
using RandomFact_Slack.Infrastructure.Options;

namespace RandomFact_Slack.Infrastructure.Service
{
    public interface IRandomFactApiService
    {
        Task<string> GetFact();
    }

    public class RandomFactApiService : IRandomFactApiService
    {
       // private readonly IHttpClientFactory _httpClient;
        private readonly FactOptions _factOptions;

        public RandomFactApiService(FactOptions factOptions)
        {
            _factOptions = factOptions;
           // _httpClient = httpClient;
        }

        public async Task<string> GetFact()
        {
            //using (var httpClient = _httpClient.)
            //{

            //}
            return "Hello";
        }
    }
}