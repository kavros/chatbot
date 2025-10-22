using System.ComponentModel;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Tools
{
    public class TavilySearchTool(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<TavilySearchTool> logger)
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly string _apiKey = Environment.GetEnvironmentVariable("TavilyAPIKey") ?? string.Empty;
        private readonly ILogger<TavilySearchTool> _logger = logger;
        private const string Endpoint = "https://api.tavily.com/search";

        [Description("Search the web using Tavily.")]
        public async Task<string> SearchTavilyAsync([Description("The user's query")] string query)
        {
            _logger.LogInformation("Starting Tavily search for query: {Query}", query);

            try
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

                var requestBody = new { query };
                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                _logger.LogInformation("Sending request to Tavily API at {Endpoint} {Content}", Endpoint, content);
                var response = await client.PostAsync(Endpoint, content);

                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Received successful response from Tavily API.");
                return responseContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching Tavily with query: {Query}", query);
                throw;
            }
        }
    }
}