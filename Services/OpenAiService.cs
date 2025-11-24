using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ApiAiBlazorLab.Services
{
    public class OpenAiService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public OpenAiService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        public async Task<string> AskAsync(string prompt)
        {
            var apiKey = _config["OpenAI:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                return "ERROR: OpenAI key not found.";

            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            var payload = new
            {
                model = "gpt-4o-mini",
                input = prompt
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Responses API endpoint
            var res = await _http.PostAsync("https://api.openai.com/v1/responses", content);
            var body = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
                return $"OpenAI error: {res.StatusCode}\n{body}";

            using var doc = JsonDocument.Parse(body);

            // Grab first text output safely
            var outputText = doc.RootElement
                .GetProperty("output")[0]
                .GetProperty("content")[0]
                .GetProperty("text")
                .GetString();

            return outputText ?? "(no text returned)";
        }
    }
}
