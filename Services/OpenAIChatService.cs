using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ApiAiBlazorLab.Services
{
    public class OpenAIChatService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public OpenAIChatService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        public async Task<string> GetCompletionAsync(string userPrompt)
        {
            var apiKey = _config["OpenAI:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
                return "ERROR: No OpenAI API key found in user-secrets.";

            var requestBody = new
            {
                model = "gpt-4o-mini",
                input = userPrompt
            };

            var requestJson = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/responses");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            request.Content = content;

            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                return $"OpenAI error: {response.StatusCode}\n{errorText}";
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            // Responses API: output[] -> content[] -> text
            if (doc.RootElement.TryGetProperty("output", out var outputArray))
            {
                var sb = new StringBuilder();

                foreach (var outputItem in outputArray.EnumerateArray())
                {
                    if (!outputItem.TryGetProperty("content", out var contentArray))
                        continue;

                    foreach (var contentItem in contentArray.EnumerateArray())
                    {
                        if (contentItem.TryGetProperty("type", out var typeProp) &&
                            typeProp.GetString() == "output_text" &&
                            contentItem.TryGetProperty("text", out var textProp))
                        {
                            sb.Append(textProp.GetString());
                        }
                    }
                }

                var finalText = sb.ToString();
                if (!string.IsNullOrWhiteSpace(finalText))
                    return finalText;
            }

            return "No response text found.";
        }

    }
}
