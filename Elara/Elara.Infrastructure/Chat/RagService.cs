using Elara.Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Elara.Infrastructure.Chat
{
    public class RagService : IRagService
    {
        private readonly HttpClient _httpClient;
        private readonly RagApiSettings _settings;

        public RagService(HttpClient httpClient, IOptions<RagApiSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<string> GetRelevantChunksAsync(string query,string subject,CancellationToken cancellationToken = default)
        {
            var url = $"{_settings.BaseUrl.TrimEnd('/')}/get-relevant-chunks";

            var requestBody = new { query, subject = subject.ToLowerInvariant() };
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(requestBody, options);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new Exception($"RAG ERROR: {errorMsg}");
            }

            var responseJson = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);

            var chunks = responseJson.GetProperty("chunks").EnumerateArray()
                .Select(chunk => chunk.GetProperty("text").GetString())
                .Where(t => !string.IsNullOrEmpty(t));

            return string.Join("\n\n", chunks);
        }
    }
}
