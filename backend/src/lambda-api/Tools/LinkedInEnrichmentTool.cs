using System;
using System.ComponentModel;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Project1.Tools
{
    public class LinkedInEnrichmentTool(
        TavilySearchTool tavilySearchTool,
        ScrapinLinkedInTool scrapinLinkedInTool,
        ILogger<LinkedInEnrichmentTool> logger)
    {
        private readonly TavilySearchTool _tavilySearchTool = tavilySearchTool;
        private readonly ScrapinLinkedInTool _scrapinLinkedInTool = scrapinLinkedInTool;
        private readonly ILogger<LinkedInEnrichmentTool> _logger = logger;

        [Description("Search for a LinkedIn profile using Tavily and enrich it using Scrapin.")]
        public async Task<string> EnrichLinkedInProfileAsync([Description("The fullname of the person along with key personal details ")] string query)
        {
            _logger.LogInformation("Starting LinkedIn enrichment process for query: {Query}", query);

            try
            {
                // Step 1: Search for LinkedIn URL using Tavily
                _logger.LogInformation("Searching for LinkedIn URL using Tavily.");
                var tavilyResult = await _tavilySearchTool.SearchTavilyAsync(query + " LinkedIn URL");
                _logger.LogInformation("Received Tavily search result.");

                // Step 2: Extract LinkedIn URL from Tavily result
                string linkedInUrl = ExtractLinkedInUrl(tavilyResult);
                if (string.IsNullOrEmpty(linkedInUrl))
                {
                    _logger.LogWarning("LinkedIn URL not found in Tavily response for query: {Query}", query);
                    throw new InvalidOperationException("LinkedIn URL not found in Tavily response.");
                }
                _logger.LogInformation("Extracted LinkedIn URL: {LinkedInUrl}", linkedInUrl);

                // Step 3: Get profile info from Scrapin
                _logger.LogInformation("Fetching LinkedIn profile info using Scrapin API.");
                var profileInfo = await _scrapinLinkedInTool.GetLinkedInProfileInfoAsync(linkedInUrl);
                _logger.LogInformation("Successfully fetched LinkedIn profile info.");

                return profileInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the LinkedIn enrichment process for query: {Query}", query);
                throw;
            }
        }

        private static string ExtractLinkedInUrl(string text)
        {
            // Regex to match LinkedIn profile URLs
            var regex = new Regex(@"https:\/\/([a-z]{2,3}\.)?linkedin\.com\/in\/[a-zA-Z0-9\-_%]+", RegexOptions.IgnoreCase);
            var match = regex.Match(text);
            return match.Success ? match.Value : null;
        }
    }
}