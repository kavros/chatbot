using System.ComponentModel;

namespace Tools
{
    public class ScrapinLinkedInTool
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey;
        private const string Endpoint = "https://api.scrapin.io/v1/enrichment/profile";

        public ScrapinLinkedInTool(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiKey = Environment.GetEnvironmentVariable("ScrapintAPIKey") ?? throw new InvalidOperationException("Failed to load Scrapin API key");
        }

        [Description("Get LinkedIn profile info using Scrapin API.")]
        public async Task<string> GetLinkedInProfileInfoAsync(string linkedInUrl)
        {
            var client = _httpClientFactory.CreateClient();

            // Construct the request payload
            var url = linkedInUrl;
            var payload = new
            {
                linkedInUrl = url,
                includes = new
                {
                    includeCompany = true,
                    includeSummary = true,
                    includeFollowersCount = true,
                    includeCreationDate = true,
                    includeSkills = true,
                    includeLanguages = true,
                    includeExperience = true,
                    includeEducation = true,
                    includeCertifications = true
                }
            };

            // Create the HTTP request
            var request = new HttpRequestMessage(HttpMethod.Post, Endpoint)
            {
                Content = JsonContent.Create(payload) // JsonContent automatically sets the Content-Type header
            };

            // Add the API key to the headers
            request.Headers.Add("x-api-key", _apiKey);

            // Send the request
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // Read and return the response content
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
    }
}