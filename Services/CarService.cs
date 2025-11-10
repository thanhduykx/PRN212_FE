using AssignmentPRN212.DTO;
using AssignmentPRN212.Models;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace AssignmentPRN212.Services
{
    public class CarService
    {
        private readonly ApiService _api;

        public CarService(ApiService api) => _api = api;
        public async Task<List<CarDTO>> GetAllCarsAsync()
        {
            var json = await _api.GetStringAsync("Car");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<DataWrapper<CarDTO>>>(json, options);
            return apiResponse?.Data?.Values ?? new List<CarDTO>();

        }
    }
}
