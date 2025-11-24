using System.Net.Http.Json;
using ApiAiBlazorLab.Models;

namespace ApiAiBlazorLab.Services
{
    public class CatFactService
    {
        private readonly HttpClient _http;

        public CatFactService(HttpClient http)
        {
            _http = http;
        }

        public async Task<string> GetRandomFactAsync()
        {
            var url = "https://catfact.ninja/fact";
            var json = await _http.GetFromJsonAsync<CatFact>(url);

            return json?.fact ?? "No cat fact received.";
        }
    }
}
