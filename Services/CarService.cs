using AssignmentPRN212.DTO;
using AssignmentPRN212.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssignmentPRN212.Services
{
    public class CarService
    {
        private readonly ApiService _apiService;

        public CarService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<CarDTO>> GetAllCarsAsync()
        {
            var response = await _apiService.GetAsync<ApiResponse<DataWrapper<CarDTO>>>("Car");
            return response.Data?.Values ?? new List<CarDTO>();
        }

        public async Task<CarDTO?> AddCarAsync(CarDTO car)
        {
            var response = await _apiService.PostAsync<CarDTO, ApiResponse<CarDTO>>("Car", car);
            return response.Data;
        }

        public async Task<CarDTO?> UpdateCarAsync(CarDTO car)
        {
            var response = await _apiService.PutAsync<CarDTO, ApiResponse<CarDTO>>($"Car/{car.Id}", car);
            return response.Data;
        }

        public async Task<bool> DeleteCarAsync(int id)
        {
            var response = await _apiService.DeleteAsync<ApiResponse<object>>($"Car/{id}");
            return response != null;
        }
    }
}
