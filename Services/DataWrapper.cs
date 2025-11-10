using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AssignmentPRN212.Services
{
    public class DataWrapper<T>
    {
        [JsonPropertyName("$values")]
        public List<T> Values { get; set; } = new List<T>();
    }
}
