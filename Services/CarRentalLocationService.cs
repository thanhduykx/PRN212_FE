using AssignmentPRN212.DTO;
using AssignmentPRN212.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssignmentPRN212.Services
{
    public class CarRentalLocationService
    {
        private readonly ApiService _apiService;

        public CarRentalLocationService(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET /api/CarRentalLocation/GetAll - Lấy tất cả (AllowAnonymous)
        public async Task<List<CarRentalLocationDTO>> GetAllAsync()
        {
            try
            {
                // Sử dụng ApiResponse<DataWrapper> format
                var response = await _apiService.GetAsync<ApiResponse<DataWrapper<CarRentalLocationDTO>>>("CarRentalLocation/GetAll");
                if (response?.Data?.Values != null && response.Data.Values.Count > 0)
                    return response.Data.Values;

                // Fallback: ApiResponse<List>
                var listResponse = await _apiService.GetAsync<ApiResponse<List<CarRentalLocationDTO>>>("CarRentalLocation/GetAll");
                if (listResponse?.Data != null && listResponse.Data.Count > 0)
                    return listResponse.Data;

                return new List<CarRentalLocationDTO>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetAllAsync error: {ex.Message}");
                return new List<CarRentalLocationDTO>();
            }
        }

        // GET /api/CarRentalLocation/GetById - Lấy theo Id
        public async Task<CarRentalLocationDTO?> GetByIdAsync(int id)
        {
            // Sử dụng ApiResponse format
            var response = await _apiService.GetAsync<ApiResponse<CarRentalLocationDTO>>($"CarRentalLocation/GetById?id={id}");
            return response?.Data;
        }

        // GET /api/CarRentalLocation/GetByCarId - Lấy theo CarId
        public async Task<List<CarRentalLocationDTO>> GetByCarIdAsync(int carId)
        {
            // Sử dụng ApiResponse format
            var response = await _apiService.GetAsync<ApiResponse<DataWrapper<CarRentalLocationDTO>>>($"CarRentalLocation/GetByCarId?carId={carId}");
            if (response?.Data?.Values != null)
                return response.Data.Values;

            // Fallback: ApiResponse<List>
            var listResponse = await _apiService.GetAsync<ApiResponse<List<CarRentalLocationDTO>>>($"CarRentalLocation/GetByCarId?carId={carId}");
            return listResponse?.Data ?? new List<CarRentalLocationDTO>();
        }

        // GET /api/CarRentalLocation/GetByRentalLocationId - Lấy theo LocationId
        public async Task<List<CarRentalLocationDTO>> GetByRentalLocationIdAsync(int locationId)
        {
            // Sử dụng ApiResponse format
            var response = await _apiService.GetAsync<ApiResponse<DataWrapper<CarRentalLocationDTO>>>($"CarRentalLocation/GetByRentalLocationId?locationId={locationId}");
            if (response?.Data?.Values != null)
                return response.Data.Values;

            // Fallback: ApiResponse<List>
            var listResponse = await _apiService.GetAsync<ApiResponse<List<CarRentalLocationDTO>>>($"CarRentalLocation/GetByRentalLocationId?locationId={locationId}");
            return listResponse?.Data ?? new List<CarRentalLocationDTO>();
        }

        // POST /api/CarRentalLocation/Create - Tạo mới
        public async Task<CarRentalLocationDTO?> CreateAsync(CreateCarRentalLocationDTO request)
        {
            try
            {
                // Sử dụng ApiResponse format
                var response = await _apiService.PostAsync<CreateCarRentalLocationDTO, ApiResponse<CarRentalLocationDTO>>("CarRentalLocation/Create", request);
                if (response?.Data != null)
                {
                    System.Diagnostics.Debug.WriteLine($"CarRentalLocation created: Id={response.Data.Id}, CarId={response.Data.CarId}, LocationId={response.Data.LocationId}");
                    return response.Data;
                }
                
                // Nếu response.Data là null, có thể có lỗi
                System.Diagnostics.Debug.WriteLine($"WARNING: CarRentalLocation CreateAsync returned null Data. CarId={request.CarId}, LocationId={request.LocationId}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CarRentalLocation CreateAsync error: {ex.Message}");
                throw; // Re-throw để caller có thể xử lý
            }
        }

        // PUT /api/CarRentalLocation/Update - Cập nhật
        public async Task<CarRentalLocationDTO?> UpdateAsync(UpdateCarRentalLocationDTO request)
        {
            // Sử dụng ApiResponse format
            var response = await _apiService.PutAsync<UpdateCarRentalLocationDTO, ApiResponse<CarRentalLocationDTO>>("CarRentalLocation/Update", request);
            return response?.Data;
        }

        // DELETE /api/CarRentalLocation/{id} - Xóa
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<ApiResponse<object>>($"CarRentalLocation/{id}");
                return response != null;
            }
            catch
            {
                return false;
            }
        }

        // GET /api/CarRentalLocation/GetByCarAndLocationId - Lấy theo CarId và LocationId
        // NOTE: Endpoint này có thể không tồn tại hoặc không hỗ trợ GET (405 Method Not Allowed)
        // Thay vào đó, dùng GetByCarIdAsync và filter trong code
        // public async Task<CarRentalLocationDTO?> GetByCarAndLocationIdAsync(int carId, int locationId)
        // {
        //     // Sử dụng ApiResponse format
        //     var response = await _apiService.GetAsync<ApiResponse<CarRentalLocationDTO>>($"CarRentalLocation/GetByCarAndLocationId?carId={carId}&locationId={locationId}");
        //     return response?.Data;
        // }
    }
}

