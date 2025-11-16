using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AssignmentPRN212.DTOs;

namespace AssignmentPRN212.Services
{
    public class ApiService
    {
        private readonly HttpClient _client;
        private string _token;

        public ApiService(string baseUrl)
        {
            _client = new HttpClient { BaseAddress = new Uri(baseUrl) };
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
            return await response.Content.ReadFromJsonAsync<T>();
        }

        // POST dữ liệu và parse JSON response
        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            var response = await _client.PostAsJsonAsync(endpoint, data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        // PUT dữ liệu và parse JSON response
        public async Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            var response = await _client.PutAsJsonAsync(endpoint, data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        // DELETE và parse JSON response
        public async Task<TResponse> DeleteAsync<TResponse>(string endpoint)
        {
            var response = await _client.DeleteAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        // GET raw string
        public async Task<string> GetStringAsync(string endpoint)
        {
            var response = await _client.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<IEnumerable<FeedbackDTO>> GetAllFeedbacksAsync()
        {
            var response = await _client.GetAsync("api/Feedback");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<FeedbackDTO>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<FeedbackDTO> AddFeedbackAsync(FeedbackDTO feedback)
        {
            var json = JsonSerializer.Serialize(feedback);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/Feedback", content);
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<FeedbackDTO>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<FeedbackDTO> UpdateFeedbackAsync(FeedbackDTO feedback)
        {
            var json = JsonSerializer.Serialize(feedback);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"api/Feedback/{feedback.Id}", content);
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<FeedbackDTO>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<bool> DeleteFeedbackAsync(int id)
        {
            var response = await _client.DeleteAsync($"api/Feedback/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<RentalLocationDTO>> GetAllRentalLocationsAsync()
        {
            var response = await _client.GetAsync("api/RentalLocation");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<RentalLocationDTO>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<RentalLocationDTO> AddRentalLocationAsync(RentalLocationDTO location)
        {
            var json = JsonSerializer.Serialize(location);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/RentalLocation", content);
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<RentalLocationDTO>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<RentalLocationDTO> UpdateRentalLocationAsync(RentalLocationDTO location)
        {
            var json = JsonSerializer.Serialize(location);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"api/RentalLocation/{location.Id}", content);
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<RentalLocationDTO>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<bool> DeleteRentalLocationAsync(int id)
        {
            var response = await _client.DeleteAsync($"api/RentalLocation/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
