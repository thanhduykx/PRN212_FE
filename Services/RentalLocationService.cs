using AssignmentPRN212.DTO;
using AssignmentPRN212.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssignmentPRN212.Services
{
    public class RentalLocationService
    {
        private readonly ApiService _apiService;

        public RentalLocationService(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET /api/RentalLocation/GetAll - Lấy tất cả địa điểm (AllowAnonymous)
        public async Task<List<RentalLocationDTO>> GetAllAsync()
        {
            try
            {
                // Thử parse với CarResponse format
                try
                {
                    var carResponse = await _apiService.GetAsync<CarResponse<RentalLocationDTO>>("RentalLocation/GetAll");
                    if (carResponse?.Values != null && carResponse.Values.Count > 0)
                        return carResponse.Values;
                }
                catch { }

                // Thử parse với DataWrapper format
                try
                {
                    var dataWrapper = await _apiService.GetAsync<DataWrapper<RentalLocationDTO>>("RentalLocation/GetAll");
                    if (dataWrapper?.Values != null && dataWrapper.Values.Count > 0)
                        return dataWrapper.Values;
                }
                catch { }

                // Thử parse với ApiResponse<DataWrapper> format
                try
                {
                    var response = await _apiService.GetAsync<ApiResponse<DataWrapper<RentalLocationDTO>>>("RentalLocation/GetAll");
                    if (response?.Data?.Values != null && response.Data.Values.Count > 0)
                        return response.Data.Values;
                }
                catch { }

                // Thử parse trực tiếp như List
                try
                {
                    var directResponse = await _apiService.GetAsync<List<RentalLocationDTO>>("RentalLocation/GetAll");
                    if (directResponse != null && directResponse.Count > 0)
                        return directResponse;
                }
                catch { }

                return new List<RentalLocationDTO>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetAllAsync error: {ex.Message}");
                return new List<RentalLocationDTO>();
            }
        }

        // GET /api/RentalLocation/GetAllStaffByLocationId - Lấy staff theo location
        public async Task<List<UserDTO>> GetAllStaffByLocationIdAsync(int locationId)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<DataWrapper<UserDTO>>>($"RentalLocation/GetAllStaffByLocationId?locationId={locationId}");
                return response?.Data?.Values ?? new List<UserDTO>();
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.GetAsync<List<UserDTO>>($"RentalLocation/GetAllStaffByLocationId?locationId={locationId}");
                    return directResponse ?? new List<UserDTO>();
                }
                catch
                {
                    return new List<UserDTO>();
                }
            }
        }

        // GET /api/RentalLocation/GetById - Lấy địa điểm theo Id
        public async Task<RentalLocationDTO?> GetByIdAsync(int id)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<RentalLocationDTO>>($"RentalLocation/GetById?id={id}");
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.GetAsync<RentalLocationDTO>($"RentalLocation/GetById?id={id}");
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // POST /api/RentalLocation/Create - Tạo địa điểm mới
        public async Task<RentalLocationDTO?> CreateAsync(CreateRentalLocationDTO request)
        {
            try
            {
                var response = await _apiService.PostAsync<CreateRentalLocationDTO, ApiResponse<RentalLocationDTO>>("RentalLocation/Create", request);
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.PostAsync<CreateRentalLocationDTO, RentalLocationDTO>("RentalLocation/Create", request);
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // PUT /api/RentalLocation/Update - Cập nhật địa điểm
        public async Task<RentalLocationDTO?> UpdateAsync(UpdateRentalLocationDTO request)
        {
            try
            {
                var response = await _apiService.PutAsync<UpdateRentalLocationDTO, ApiResponse<RentalLocationDTO>>("RentalLocation/Update", request);
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.PutAsync<UpdateRentalLocationDTO, RentalLocationDTO>("RentalLocation/Update", request);
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // DELETE /api/RentalLocation/{id} - Xóa địa điểm
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<ApiResponse<object>>($"RentalLocation/{id}");
                return response != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
