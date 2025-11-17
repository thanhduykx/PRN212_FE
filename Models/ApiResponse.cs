using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AssignmentPRN212.Models
{
    public class ApiResponse<T>
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public T? Data { get; set; }

        // Capture các properties không biết trước (như $id) và ignore chúng
        [JsonExtensionData]
        public Dictionary<string, object>? ExtensionData { get; set; }
    }

    public class DataWrapper<T>
    {
        [JsonPropertyName("$values")]
        public List<T> Values { get; set; } = new();

        // Capture các properties không biết trước (như $id) và ignore chúng
        [JsonExtensionData]
        public Dictionary<string, object>? ExtensionData { get; set; }
    }

    // CarResponse format: { $id: string, $values: Car[] }
    public class CarResponse<T>
    {
        [JsonPropertyName("$id")]
        public string? Id { get; set; }

        [JsonPropertyName("$values")]
        public List<T> Values { get; set; } = new();
    }

    // Backend response format: { message: string, data: T }
    public class MessageDataResponse<T>
    {
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("data")]
        public T? Data { get; set; }
    }
}
