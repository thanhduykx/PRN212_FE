using AssignmentPRN212.DTO;
using AssignmentPRN212.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssignmentPRN212.Services
{
    public class CitizenIdService
    {
        private readonly ApiService _apiService;

        public CitizenIdService(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET /api/CitizenId/GetAll
        public async Task<List<CitizenIdDTO>> GetAllAsync()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<DataWrapper<CitizenIdDTO>>>("CitizenId/GetAll");
                return response?.Data?.Values ?? new List<CitizenIdDTO>();
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.GetAsync<List<CitizenIdDTO>>("CitizenId/GetAll");
                    return directResponse ?? new List<CitizenIdDTO>();
                }
                catch
                {
                    return new List<CitizenIdDTO>();
                }
            }
        }

        // GET /api/CitizenId/GetById
        public async Task<CitizenIdDTO?> GetByIdAsync(int id)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<CitizenIdDTO>>($"CitizenId/GetById?id={id}");
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.GetAsync<CitizenIdDTO>($"CitizenId/GetById?id={id}");
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // GET /api/CitizenId/GetByOrderId
        public async Task<CitizenIdDTO?> GetByOrderIdAsync(int orderId)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<CitizenIdDTO>>($"CitizenId/GetByOrderId?orderId={orderId}");
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.GetAsync<CitizenIdDTO>($"CitizenId/GetByOrderId?orderId={orderId}");
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // POST /api/CitizenId/Create
        public async Task<CitizenIdDTO?> CreateAsync(CreateCitizenIdDTO request)
        {
            try
            {
                var response = await _apiService.PostAsync<CreateCitizenIdDTO, ApiResponse<CitizenIdDTO>>("CitizenId/Create", request);
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.PostAsync<CreateCitizenIdDTO, CitizenIdDTO>("CitizenId/Create", request);
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // PUT /api/CitizenId/UpdateCitizenIdStatus
        public async Task<CitizenIdDTO?> UpdateStatusAsync(UpdateCitizenIdStatusDTO request)
        {
            try
            {
                var response = await _apiService.PutAsync<UpdateCitizenIdStatusDTO, ApiResponse<CitizenIdDTO>>("CitizenId/UpdateCitizenIdStatus", request);
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.PutAsync<UpdateCitizenIdStatusDTO, CitizenIdDTO>("CitizenId/UpdateCitizenIdStatus", request);
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // PUT /api/CitizenId/UpdateCitizenIdInfo
        public async Task<CitizenIdDTO?> UpdateInfoAsync(UpdateCitizenIdInfoDTO request)
        {
            try
            {
                var response = await _apiService.PutAsync<UpdateCitizenIdInfoDTO, ApiResponse<CitizenIdDTO>>("CitizenId/UpdateCitizenIdInfo", request);
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.PutAsync<UpdateCitizenIdInfoDTO, CitizenIdDTO>("CitizenId/UpdateCitizenIdInfo", request);
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

