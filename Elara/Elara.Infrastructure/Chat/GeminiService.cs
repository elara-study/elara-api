using Elara.Application.Common.Interfaces;
using Elara.Application.Features.Chat;
using Elara.Domain.Enums;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Elara.Infrastructure.Chat
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly GeminiSettings _settings;

        private const string PrimaryModel = "gemini-3-flash-preview";
        private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta";

        public GeminiService(HttpClient httpClient, IOptions<GeminiSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<string> GenerateResponseAsync(string userMessage,string ragContext,IEnumerable<ChatMessageDto> conversationHistory,CancellationToken cancellationToken = default)
        {
            var prompt = BuildPrompt(userMessage, ragContext, conversationHistory);
            var requestBody = new
            {
                model = PrimaryModel,
                input = prompt
            };

            var result = await CallGeminiAsync(requestBody, cancellationToken);

            return result ?? throw new InvalidOperationException("Gemini API failed to return a response.");
        }

        private async Task<string?> CallGeminiAsync(object requestBody, CancellationToken cancellationToken)
        {
            var url = $"{BaseUrl}/interactions?key={_settings.ApiKey}";
            var json = JsonSerializer.Serialize(requestBody);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException($"Gemini API error: {response.StatusCode} - {errorMsg}");
            }

            var responseJson = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);

            var outputs = responseJson.GetProperty("outputs");
            return outputs[outputs.GetArrayLength() - 1].GetProperty("text").GetString();
        }

        private static string BuildPrompt(
            string userMessage,
            string ragContext,
            IEnumerable<ChatMessageDto> history)
        {
            var sb = new StringBuilder();

            sb.AppendLine("You are a friendly, natural educational AI assistant for students in Egypt, specifically tailored for the 3rd Secondary (Thanaweya Amma) curriculum. Start your answers directly.");
            sb.AppendLine("CRITICAL RULES:");
            sb.AppendLine("- Prioritize the provided curriculum context to answer the student's questions.");
            sb.AppendLine("- If the answer is NOT found in the provided context, gracefully rely on your general educational knowledge of the Egyptian 3rd Secondary (Thanaweya Amma) curriculum to provide an accurate answer.");
            sb.AppendLine("- NEVER use phrases like 'Based on the provided context', 'According to my knowledge', 'Based on the Egyptian curriculum' or 'In the text'. Answer seamlessly as if the knowledge is naturally yours.");
            sb.AppendLine();
            sb.AppendLine("=== CURRICULUM CONTEXT ===");
            sb.AppendLine(ragContext);
            sb.AppendLine();

            var historyList = history.ToList();
            if (historyList.Count > 0)
            {
                sb.AppendLine("=== CONVERSATION HISTORY ===");
                foreach (var msg in historyList)
                {
                    var label = msg.Role == MessageRole.Student ? "Student" : "AI";
                    sb.AppendLine($"{label}: {msg.Content}");
                }
                sb.AppendLine();
            }

            sb.AppendLine("=== NEW QUESTION ===");
            sb.AppendLine($"Student: {userMessage}");
            sb.AppendLine("AI:");

            return sb.ToString();
        }
    }
}
