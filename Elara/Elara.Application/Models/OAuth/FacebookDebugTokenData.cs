using System.Text.Json.Serialization;

namespace Elara.Application.Models.OAuth
{
    public class FacebookDebugTokenData
    {
        [JsonPropertyName("app_id")]
        public string AppId   { get; set; } = string.Empty;
        
        [JsonPropertyName("is_valid")]
        public bool   IsValid { get; set; }
    }
}
