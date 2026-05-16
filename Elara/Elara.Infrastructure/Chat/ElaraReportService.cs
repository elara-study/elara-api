using System.Net.Http.Json;
using System.Text.Json;
using Elara.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Elara.Infrastructure.Chat
{
    public class ElaraReportService : IElaraReportService
    {
        private readonly HttpClient _httpClient;
        private readonly ElaraReportSettings _settings;
        private readonly ILogger<ElaraReportService> _logger;

        public ElaraReportService(
            HttpClient httpClient,
            IOptions<ElaraReportSettings> settings,
            ILogger<ElaraReportService> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<string> AnalyzeConversationAsync(
            string transcript, CancellationToken cancellationToken = default)
        {
            var requestBody = new { transcript };
            var response = await _httpClient.PostAsJsonAsync(
                _settings.EndpointUrl, requestBody, cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var status = root.GetProperty("status").GetString();
            if (status != "success")
                throw new InvalidOperationException(
                    $"Elara Report API returned status: {status}");

            return root.GetProperty("report").GetString()
                ?? throw new InvalidOperationException(
                    "Elara Report API returned null report");
        }
    }
}
