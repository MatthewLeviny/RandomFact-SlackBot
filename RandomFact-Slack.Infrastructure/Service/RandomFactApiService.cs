using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using RandomFact_Slack.Core.Dto;
using RandomFact_Slack.Infrastructure.Options;

namespace RandomFact_Slack.Infrastructure.Service
{
    public interface IRandomFactApiService
    {
        Task<FactResponse> GetFact();
    }

    public class RandomFactApiService : IRandomFactApiService
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly FactOptions _factOptions;

        public RandomFactApiService(FactOptions factOptions, IHttpClientFactory httpClient)
        {
            _factOptions = factOptions;
            _httpClient = httpClient;
        }

        public async Task<FactResponse> GetFact()
        {
            using var httpClient = _httpClient.CreateClient("FactApi");
            var response = await httpClient.GetAsync("/random.json?language=en");
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<FactResponse>(await response.Content.ReadAsStringAsync());
            }
            throw new HttpRequestException($"Error {response.StatusCode} {response.Content} when querying {httpClient.BaseAddress}endpoint.");
        }
    }
}