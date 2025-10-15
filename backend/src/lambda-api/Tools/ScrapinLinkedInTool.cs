using System.ComponentModel;

namespace Project1.Tools
{
    public class ScrapinLinkedInTool
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey;
        private const string Endpoint = "https://api.scrapin.io/enrichment/profile";
        public ScrapinLinkedInTool(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiKey =Environment.GetEnvironmentVariable("ScrapintAPIKey") ?? throw new InvalidOperationException("Failed to load Scrapin API key");
        }

        [Description("Get LinkedIn profile info using Scrapin API.")]
        public async Task<string> GetLinkedInProfileInfoAsync(string linkedInUrl)
        {
            var client = _httpClientFactory.CreateClient();

            var requestUri = $"{Endpoint}?apikey={Uri.EscapeDataString(_apiKey)}&linkedInUrl={Uri.EscapeDataString(linkedInUrl)}";
            var response = await client.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
    }
}