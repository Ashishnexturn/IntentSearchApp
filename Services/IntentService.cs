using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

public class IntentService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _http;
    private readonly ILogger<IntentService> _logger;

    public IntentService(IConfiguration config, ILogger<IntentService> logger)
    {
        _config = config;
        _http = new HttpClient();
        _logger = logger;
    }

    public async Task<IntentResult> DetectIntent(string userQuery)
    {
        try
        {
            var apiKey = _config["Groq:ApiKey"];
            var baseUrl = _config["Groq:BaseUrl"];

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(baseUrl))
            {
                _logger.LogError("Missing configuration: ApiKey={HasApiKey}, BaseUrl={HasBaseUrl}",
                    !string.IsNullOrEmpty(apiKey), !string.IsNullOrEmpty(baseUrl));
                throw new InvalidOperationException("Groq API configuration is missing");
            }

            _logger.LogInformation("Starting intent detection for query: {Query}", userQuery);

            _http.DefaultRequestHeaders.Clear();
            _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var prompt = $@"
Analyze the user query and classify the intent to help search for products.
Possible intents: party, lunch, party_lunch, general, shopping
Extract meaningful keyword(s) from the query.

Return JSON ONLY (no markdown formatting):
{{
  ""intent"": ""[party|lunch|party_lunch|general|shopping]"",
  ""keyword"": ""[main keyword or product category]"",
  ""category"": ""[snacks|food|drinks|general]""
}}

Examples:
- Query: ""party lunch"" → {{""intent"": ""party_lunch"", ""keyword"": ""party lunch"", ""category"": ""snacks""}}
- Query: ""I need snacks for party"" → {{""intent"": ""party"", ""keyword"": ""snacks"", ""category"": ""snacks""}}
- Query: ""cold drinks for gathering"" → {{""intent"": ""party"", ""keyword"": ""cold drinks"", ""category"": ""drinks""}}
- Query: ""lunch items"" → {{""intent"": ""lunch"", ""keyword"": ""lunch items"", ""category"": ""food""}}

User Query: {userQuery}
";

            var body = new
            {
                model = "openai/gpt-oss-120b",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(body),
                Encoding.UTF8,
                "application/json"
            );

            _logger.LogDebug("Sending request to: {BaseUrl}/chat/completions", baseUrl);

            var response = await _http.PostAsync(
                $"{baseUrl}/chat/completions",
                content
            );

            _logger.LogInformation("API Response Status: {StatusCode}", response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("API Error ({StatusCode}): {Response}", response.StatusCode, json);
                throw new HttpRequestException($"API returned status {response.StatusCode}");
            }

            _logger.LogDebug("Raw API Response: {Response}", json);

            dynamic result = JsonConvert.DeserializeObject(json);

            if (result?.choices == null || result.choices.Count == 0)
            {
                _logger.LogError("Invalid response structure: no choices found in response");
                throw new InvalidOperationException("API response missing choices");
            }

            string message = result.choices[0].message.content;

            _logger.LogDebug("Extracted message: {Message}", message);

            var intentResult = JsonConvert.DeserializeObject<IntentResult>(message);

            _logger.LogInformation("Successfully parsed intent. Intent={Intent}, Keyword={Keyword}, Category={Category}",
                intentResult.Intent, intentResult.Keyword, intentResult.Category);

            return intentResult;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error while detecting intent for query: {Query}", userQuery);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting intent for query: {Query}", userQuery);
            throw;
        }
    }
}
