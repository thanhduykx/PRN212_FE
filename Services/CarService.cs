using AssignmentPRN212.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssignmentPRN212.Services
{
    public class CarService
    {
        private readonly ApiService _api;

        public CarService(ApiService api)
        {
            _api = api;
        }

        // Lấy danh sách xe
        public async Task<List<CarDTO>> GetAllCarsAsync()
        {
            // API trả về: { message, data: { $values: [...] } }
            var response = await _api.GetAsync<ApiResponse<DataWrapper<CarDTO>>>("Car");
            return response?.Data?.Values ?? new List<CarDTO>();
        }

        public async Task<CarDTO?> AddCarAsync(CreateCarDTO car)
        {
            var response = await _api.PostAsync<CreateCarDTO, ApiResponse<CarDTO>>("Car", car);
            return response?.Data;
        }

        public async Task<CarDTO?> UpdateCarAsync(UpdateCarDTO car)
        {
            var response = await _api.PutAsync<UpdateCarDTO, ApiResponse<CarDTO>>($"Car/{car.Id}", car);
            return response?.Data;
        }

        public async Task<bool> DeleteCarAsync(int id)
        {
            var response = await _api.DeleteAsync<ApiResponse<object>>($"Car/{id}");
            return response != null;
        }
    }
}
