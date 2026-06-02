using System.Text.Json.Serialization;

namespace Elara.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ResourceType
    {
        Video = 1,
        Pdf = 2,
        Image = 3
    }
}
