using AssignmentPRN212.DTO;
using AssignmentPRN212.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssignmentPRN212.Services
{
    public class CarReturnHistoryService
    {
        private readonly ApiService _apiService;

        public CarReturnHistoryService(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET /api/CarReturnHistory - Lấy tất cả lịch sử trả xe
        public async Task<List<CarReturnHistoryDTO>> GetAllAsync()
        {
            try
            {
                // Backend trả về format { message, data }
                try
                {
                    var response = await _apiService.GetAsync<ApiResponse<DataWrapper<CarReturnHistoryDTO>>>("CarReturnHistory");
                    if (response?.Data?.Values != null)
                        return response.Data.Values; // Return ngay cả khi empty list
                }
                catch (System.Text.Json.JsonException jsonEx)
                {
                    System.Diagnostics.Debug.WriteLine($"GetAllAsync ApiResponse format JsonException: {jsonEx.Message}");
                    System.Diagnostics.Debug.WriteLine($"JsonException details: Path={jsonEx.Path}, LineNumber={jsonEx.LineNumber}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"GetAllAsync ApiResponse format error: {ex.GetType().Name} - {ex.Message}");
                }

                // Thử parse với CarResponse format
                try
                {
                    var carResponse = await _apiService.GetAsync<CarResponse<CarReturnHistoryDTO>>("CarReturnHistory");
                    if (carResponse?.Values != null && carResponse.Values.Count > 0)
                        return carResponse.Values;
                }
                catch (System.Text.Json.JsonException jsonEx)
                {
                    System.Diagnostics.Debug.WriteLine($"GetAllAsync CarResponse format JsonException: {jsonEx.Message}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"GetAllAsync CarResponse format error: {ex.GetType().Name} - {ex.Message}");
                }

                // Thử parse với DataWrapper format
                try
                {
                    var dataWrapper = await _apiService.GetAsync<DataWrapper<CarReturnHistoryDTO>>("CarReturnHistory");
                    if (dataWrapper?.Values != null && dataWrapper.Values.Count > 0)
                        return dataWrapper.Values;
                }
                catch (System.Text.Json.JsonException jsonEx)
                {
                    System.Diagnostics.Debug.WriteLine($"GetAllAsync DataWrapper format JsonException: {jsonEx.Message}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"GetAllAsync DataWrapper format error: {ex.GetType().Name} - {ex.Message}");
                }

                // Thử parse trực tiếp như List (chỉ khi các format trên fail)
                // Format này sẽ fail vì backend không trả về List trực tiếp
                // Nhưng vẫn thử để đảm bảo backward compatibility
                try
                {
                    var directResponse = await _apiService.GetAsync<List<CarReturnHistoryDTO>>("CarReturnHistory");
                    if (directResponse != null)
                        return directResponse;
                }
                catch (System.Text.Json.JsonException jsonEx)
                {
                    // Expected exception - backend không trả về List trực tiếp
                    System.Diagnostics.Debug.WriteLine($"GetAllAsync List format JsonException (expected): {jsonEx.Message}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"GetAllAsync List format error: {ex.GetType().Name} - {ex.Message}");
                }

                return new List<CarReturnHistoryDTO>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetAllAsync error: {ex.GetType().Name} - {ex.Message}");
                return new List<CarReturnHistoryDTO>();
            }
        }

        // GET /api/CarReturnHistory/{id} - Lấy lịch sử trả xe theo Id
        public async Task<CarReturnHistoryDTO?> GetByIdAsync(int id)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<CarReturnHistoryDTO>>($"CarReturnHistory/{id}");
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.GetAsync<CarReturnHistoryDTO>($"CarReturnHistory/{id}");
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // POST /api/CarReturnHistory - Tạo lịch sử trả xe mới
        public async Task<CarReturnHistoryDTO?> CreateAsync(CarReturnHistoryCreateDTO request)
        {
            try
            {
                var response = await _apiService.PostAsync<CarReturnHistoryCreateDTO, ApiResponse<CarReturnHistoryDTO>>("CarReturnHistory", request);
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.PostAsync<CarReturnHistoryCreateDTO, CarReturnHistoryDTO>("CarReturnHistory", request);
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // PUT /api/CarReturnHistory/{id} - Cập nhật lịch sử trả xe
        public async Task<CarReturnHistoryDTO?> UpdateAsync(int id, CarReturnHistoryCreateDTO request)
        {
            try
            {
                var response = await _apiService.PutAsync<CarReturnHistoryCreateDTO, ApiResponse<CarReturnHistoryDTO>>($"CarReturnHistory/{id}", request);
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.PutAsync<CarReturnHistoryCreateDTO, CarReturnHistoryDTO>($"CarReturnHistory/{id}", request);
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // DELETE /api/CarReturnHistory/{id} - Xóa lịch sử trả xe
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<ApiResponse<object>>($"CarReturnHistory/{id}");
                return response != null;
            }
            catch
            {
                return false;
            }
        }
    }
}

