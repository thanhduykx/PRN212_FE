using AssignmentPRN212.DTO;
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

        // GET danh sách xe
        public async Task<List<CarDTO>> GetAllCarsAsync()
        {
            var response = await _apiService.GetAsync<ApiResponse<DataWrapper<CarDTO>>>("Car");
            return response?.Data?.Values ?? new List<CarDTO>();
        }

        // POST thêm xe
        public async Task<CarDTO?> AddCarAsync(CarDTO car)
        {
            var payload = new
            {
                car.Name,
                car.Model,
                car.Seats,
                car.SizeType,
                car.TrunkCapacity,
                car.BatteryType,
                car.BatteryDuration,
                car.RentPricePerDay,
                car.RentPricePerHour,
                car.RentPricePerDayWithDriver,
                car.RentPricePerHourWithDriver,
                car.ImageUrl,
                car.ImageUrl2,
                car.ImageUrl3,
                car.Status,
                car.IsActive,
                car.IsDeleted
            };

            var response = await _apiService.PostAsync<object, ApiResponse<CarDTO>>("Car", payload);
            return response?.Data;
        }

        // PUT cập nhật xe
        public async Task<CarDTO?> UpdateCarAsync(CarDTO car)
        {
            var now = DateTime.UtcNow;

            var payload = new
            {
                car.Name,
                car.Model,
                car.Seats,
                car.SizeType,
                car.TrunkCapacity,
                car.BatteryType,
                car.BatteryDuration,
                car.RentPricePerDay,
                car.RentPricePerHour,
                car.RentPricePerDayWithDriver,
                car.RentPricePerHourWithDriver,
                car.ImageUrl,
                car.ImageUrl2,
                car.ImageUrl3,
                car.Status,
                car.IsActive,
                car.IsDeleted,
                CreatedAt = now,
                UpdatedAt = now
            };

            var response = await _apiService.PutAsync<object, ApiResponse<CarDTO>>($"Car/{car.Id}", payload);
            return response?.Data;
        }

        // DELETE xe
        public async Task<bool> DeleteCarAsync(int id)
        {
            var response = await _apiService.DeleteAsync<ApiResponse<object>>($"Car/{id}");
            return response != null;
        }
    }
}
