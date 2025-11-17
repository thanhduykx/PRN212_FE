using AssignmentPRN212.DTO;
using AssignmentPRN212.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssignmentPRN212.Services
{
    public class DriverLicenseService
    {
        private readonly ApiService _apiService;

        public DriverLicenseService(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET /api/DriverLicense/GetAll
        public async Task<List<DriverLicenseDTO>> GetAllAsync()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<DataWrapper<DriverLicenseDTO>>>("DriverLicense/GetAll");
                return response?.Data?.Values ?? new List<DriverLicenseDTO>();
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.GetAsync<List<DriverLicenseDTO>>("DriverLicense/GetAll");
                    return directResponse ?? new List<DriverLicenseDTO>();
                }
                catch
                {
                    return new List<DriverLicenseDTO>();
                }
            }
        }

        // GET /api/DriverLicense/GetById
        public async Task<DriverLicenseDTO?> GetByIdAsync(int id)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<DriverLicenseDTO>>($"DriverLicense/GetById?id={id}");
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.GetAsync<DriverLicenseDTO>($"DriverLicense/GetById?id={id}");
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // GET /api/DriverLicense/GetByOrderId
        public async Task<DriverLicenseDTO?> GetByOrderIdAsync(int orderId)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<DriverLicenseDTO>>($"DriverLicense/GetByOrderId?orderId={orderId}");
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.GetAsync<DriverLicenseDTO>($"DriverLicense/GetByOrderId?orderId={orderId}");
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // POST /api/DriverLicense/Create
        public async Task<DriverLicenseDTO?> CreateAsync(CreateDriverLicenseDTO request)
        {
            try
            {
                var response = await _apiService.PostAsync<CreateDriverLicenseDTO, ApiResponse<DriverLicenseDTO>>("DriverLicense/Create", request);
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.PostAsync<CreateDriverLicenseDTO, DriverLicenseDTO>("DriverLicense/Create", request);
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // PUT /api/DriverLicense/UpdateDriverLicenseStatus
        public async Task<DriverLicenseDTO?> UpdateStatusAsync(UpdateDriverLicenseStatusDTO request)
        {
            try
            {
                var response = await _apiService.PutAsync<UpdateDriverLicenseStatusDTO, ApiResponse<DriverLicenseDTO>>("DriverLicense/UpdateDriverLicenseStatus", request);
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.PutAsync<UpdateDriverLicenseStatusDTO, DriverLicenseDTO>("DriverLicense/UpdateDriverLicenseStatus", request);
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // PUT /api/DriverLicense/UpdateDriverLicenseInfo
        public async Task<DriverLicenseDTO?> UpdateInfoAsync(UpdateDriverLicenseInfoDTO request)
        {
            try
            {
                var response = await _apiService.PutAsync<UpdateDriverLicenseInfoDTO, ApiResponse<DriverLicenseDTO>>("DriverLicense/UpdateDriverLicenseInfo", request);
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.PutAsync<UpdateDriverLicenseInfoDTO, DriverLicenseDTO>("DriverLicense/UpdateDriverLicenseInfo", request);
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}

