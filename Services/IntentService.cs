using System.Text;
using Newtonsoft.Json;

public class IntentService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _http;

    public IntentService(IConfiguration config)
    {
        _config = config;
        _http = new HttpClient();
    }

    public async Task<IntentResult> DetectIntent(string userQuery)
    {
        var apiKey = _config["Groq:ApiKey"];
        var baseUrl = _config["Groq:BaseUrl"];

        _http.DefaultRequestHeaders.Clear();
        _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var prompt = $@"
Classify the intent and extract keyword.
Return JSON only:
{{
  ""intent"": """",
  ""keyword"": """",
  ""category"": """"
}}

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

        var response = await _http.PostAsync(
            $"{baseUrl}/chat/completions",
            content
        );

        var json = await response.Content.ReadAsStringAsync();
        dynamic result = JsonConvert.DeserializeObject(json);

        string message = result.choices[0].message.content;

        return JsonConvert.DeserializeObject<IntentResult>(message);
    }
}