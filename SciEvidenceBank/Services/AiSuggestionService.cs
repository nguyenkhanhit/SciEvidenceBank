//csharp Services\AiSuggestionService.cs
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SciEvidenceBank.Services
{
    public class AiSuggestionService
    {
        private readonly string _endpoint;
        private readonly string _apiKey;
        private readonly HttpClient _http;

        public AiSuggestionService(string endpoint, string apiKey)
        {
            _endpoint = endpoint;
            _apiKey = apiKey;
            _http = new HttpClient();
            if (!string.IsNullOrEmpty(_apiKey))
            {
                _http.DefaultRequestHeaders.Add("Authorization", "Bearer " + _apiKey);
            }
        }

        // Example: returns raw suggestion string. Adapt to your AI provider request/response.
        public async Task<string> GetSuggestionsAsync(IEnumerable<int> researchFieldIds)
        {
            var payload = new
            {
                fieldIds = researchFieldIds
            };
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var resp = await _http.PostAsync(_endpoint, content);
            resp.EnsureSuccessStatusCode();
            var text = await resp.Content.ReadAsStringAsync();
            return text;
        }
    }
}   