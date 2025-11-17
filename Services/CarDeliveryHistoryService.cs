using AssignmentPRN212.DTO;
using AssignmentPRN212.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssignmentPRN212.Services
{
    public class CarDeliveryHistoryService
    {
        private readonly ApiService _apiService;

        public CarDeliveryHistoryService(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET /api/CarDeliveryHistory?pageIndex=1&pageSize=10 - Lấy tất cả lịch sử giao xe (có phân trang)
        public async Task<List<CarDeliveryHistoryDTO>> GetAllAsync(int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                // Sử dụng ApiResponse<PagedData> format
                var response = await _apiService.GetAsync<ApiResponse<PagedData<CarDeliveryHistoryDTO>>>($"CarDeliveryHistory?pageIndex={pageIndex}&pageSize={pageSize}");
                if (response?.Data?.Data != null && response.Data.Data.Count > 0)
                    return response.Data.Data;

                // Fallback: ApiResponse<List>
                var listResponse = await _apiService.GetAsync<ApiResponse<List<CarDeliveryHistoryDTO>>>($"CarDeliveryHistory?pageIndex={pageIndex}&pageSize={pageSize}");
                if (listResponse?.Data != null && listResponse.Data.Count > 0)
                    return listResponse.Data;

                return new List<CarDeliveryHistoryDTO>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetAllAsync error: {ex.Message}");
                return new List<CarDeliveryHistoryDTO>();
            }
        }

        // GET /api/CarDeliveryHistory/{id} - Lấy lịch sử giao xe theo Id
        public async Task<CarDeliveryHistoryDTO?> GetByIdAsync(int id)
        {
            // Sử dụng ApiResponse format
            var response = await _apiService.GetAsync<ApiResponse<CarDeliveryHistoryDTO>>($"CarDeliveryHistory/{id}");
            return response?.Data;
        }

        // POST /api/CarDeliveryHistory - Tạo lịch sử giao xe mới
        public async Task<CarDeliveryHistoryDTO?> CreateAsync(CarDeliveryHistoryCreateDTO request)
        {
            // Sử dụng ApiResponse format
            var response = await _apiService.PostAsync<CarDeliveryHistoryCreateDTO, ApiResponse<CarDeliveryHistoryDTO>>("CarDeliveryHistory", request);
            return response?.Data;
        }

        // PUT /api/CarDeliveryHistory/{id} - Cập nhật lịch sử giao xe
        public async Task<CarDeliveryHistoryDTO?> UpdateAsync(int id, CarDeliveryHistoryUpdateDTO request)
        {
            // Sử dụng ApiResponse format
            var response = await _apiService.PutAsync<CarDeliveryHistoryUpdateDTO, ApiResponse<CarDeliveryHistoryDTO>>($"CarDeliveryHistory/{id}", request);
            return response?.Data;
        }

        // DELETE /api/CarDeliveryHistory/{id} - Xóa lịch sử giao xe
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<ApiResponse<object>>($"CarDeliveryHistory/{id}");
                return response != null;
            }
            catch
            {
                return false;
            }
        }

        // GET /api/CarDeliveryHistory/Count - Lấy tổng số records (nếu có endpoint này)
        public async Task<int> CountAsync()
        {
            // Sử dụng ApiResponse format
            var response = await _apiService.GetAsync<ApiResponse<int>>("CarDeliveryHistory/Count");
            return response?.Data ?? 0;
        }
    }

    // Helper class cho PagedData
    public class PagedData<T>
    {
        public int Total { get; set; }
        public List<T> Data { get; set; } = new List<T>();
    }
}

