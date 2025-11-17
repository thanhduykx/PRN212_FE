using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace AssignmentPRN212.Services
{
    public class ApiService
    {
        private readonly HttpClient _client;
        private string _token;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService(string baseUrl)
        {
            _client = new HttpClient { BaseAddress = new Uri(baseUrl) };
            
            // Cấu hình JSON options - Property naming policy để match với backend (camelCase)
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Chuyển sang camelCase để match với backend
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = false,
                // Ignore reference tracking metadata ($id, $ref) từ backend
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
            };
        }

        public void SetToken(string token)
        {
            _token = token;
            if (!string.IsNullOrEmpty(_token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            }
            else
            {
                _client.DefaultRequestHeaders.Authorization = null;
            }
        }

        // GET và parse JSON thành object
        public async Task<T> GetAsync<T>(string endpoint)
        {
            var response = await _client.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            
            try
            {
                // Sử dụng custom JsonSerializerOptions để deserialize với camelCase
                return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
            }
            catch (JsonException jsonEx)
            {
                // Log chi tiết lỗi JSON để debug
                var content = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"JsonException in GetAsync({endpoint}): {jsonEx.Message}");
                if (!string.IsNullOrEmpty(content))
                {
                    int length = Math.Min(500, content.Length);
                    System.Diagnostics.Debug.WriteLine($"Response content (first {length} chars): {content.Substring(0, length)}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Response content is empty or null");
                }
                throw;
            }
        }

        // POST dữ liệu và parse JSON response
        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            // Serialize với custom options
            var jsonContent = JsonSerializer.Serialize(data, _jsonOptions);
            
            // Debug log để kiểm tra JSON được gửi lên
            System.Diagnostics.Debug.WriteLine($"POST {endpoint}");
            System.Diagnostics.Debug.WriteLine($"Request JSON: {jsonContent}");
            
            // Debug: Kiểm tra token có được gửi không
            if (_client.DefaultRequestHeaders.Authorization != null)
            {
                System.Diagnostics.Debug.WriteLine($"Authorization header: Bearer {_token?.Substring(0, Math.Min(20, _token?.Length ?? 0))}...");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("WARNING: No Authorization header found!");
            }
            
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _client.PostAsync(endpoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"POST {endpoint} failed: HTTP {(int)response.StatusCode} - {errorContent}");
                var httpEx = new HttpRequestException($"HTTP {(int)response.StatusCode} {response.StatusCode}: {errorContent}");
                httpEx.Data["StatusCode"] = (int)response.StatusCode;
                httpEx.Data["StatusText"] = response.StatusCode.ToString();
                httpEx.Data["ErrorContent"] = errorContent;
                throw httpEx;
            }
            
            var responseContent = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"POST {endpoint} response: {responseContent.Substring(0, Math.Min(500, responseContent.Length))}");
            
            // Sử dụng custom JsonSerializerOptions để deserialize với camelCase
            return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions);
        }

        // PUT dữ liệu và parse JSON response
        public async Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            // Serialize với custom options
            var jsonContent = JsonSerializer.Serialize(data, _jsonOptions);
            
            // Debug log để kiểm tra JSON được gửi lên
            System.Diagnostics.Debug.WriteLine($"PUT {endpoint}");
            System.Diagnostics.Debug.WriteLine($"Request JSON: {jsonContent}");
            
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _client.PutAsync(endpoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"PUT {endpoint} failed: HTTP {(int)response.StatusCode} - {errorContent}");
                var httpEx = new HttpRequestException($"HTTP {(int)response.StatusCode} {response.StatusCode}: {errorContent}");
                httpEx.Data["StatusCode"] = (int)response.StatusCode;
                httpEx.Data["StatusText"] = response.StatusCode.ToString();
                httpEx.Data["ErrorContent"] = errorContent;
                throw httpEx;
            }
            
            var responseContent = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"PUT {endpoint} response: {responseContent.Substring(0, Math.Min(500, responseContent.Length))}");
            
            // Sử dụng custom JsonSerializerOptions để deserialize với camelCase
            return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions);
        }

        // DELETE và parse JSON response
        public async Task<TResponse> DeleteAsync<TResponse>(string endpoint)
        {
            var response = await _client.DeleteAsync(endpoint);
            response.EnsureSuccessStatusCode();
            // Sử dụng custom JsonSerializerOptions để deserialize với camelCase
            return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions);
        }

        // GET raw string
        public async Task<string> GetStringAsync(string endpoint)
        {
            var response = await _client.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
