using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AssignmentPRN212.Services
{
    public class ApiResponse<T>
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public T? Data { get; set; }
    }

    public class DataWrapper<T>
    {
        [JsonPropertyName("$values")]
        public List<T> Values { get; set; } = new();
    }
}
