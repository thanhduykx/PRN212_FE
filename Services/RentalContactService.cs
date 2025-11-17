using AssignmentPRN212.DTO;
using AssignmentPRN212.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssignmentPRN212.Services
{
    public class RentalContactService
    {
        private readonly ApiService _apiService;

        public RentalContactService(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET /api/RentalContact
        public async Task<List<RentalContactDTO>> GetAllAsync()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<DataWrapper<RentalContactDTO>>>("RentalContact");
                return response?.Data?.Values ?? new List<RentalContactDTO>();
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.GetAsync<List<RentalContactDTO>>("RentalContact");
                    return directResponse ?? new List<RentalContactDTO>();
                }
                catch
                {
                    return new List<RentalContactDTO>();
                }
            }
        }

        // GET /api/RentalContact/byRentalOrder/{rentalOrderId}
        public async Task<RentalContactDTO?> GetByRentalOrderIdAsync(int rentalOrderId)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<RentalContactDTO>>($"RentalContact/byRentalOrder/{rentalOrderId}");
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.GetAsync<RentalContactDTO>($"RentalContact/byRentalOrder/{rentalOrderId}");
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // POST /api/RentalContact
        public async Task<RentalContactDTO?> CreateAsync(CreateRentalContactDTO request)
        {
            try
            {
                var response = await _apiService.PostAsync<CreateRentalContactDTO, ApiResponse<RentalContactDTO>>("RentalContact", request);
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.PostAsync<CreateRentalContactDTO, RentalContactDTO>("RentalContact", request);
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // PUT /api/RentalContact
        public async Task<RentalContactDTO?> UpdateAsync(UpdateRentalContactDTO request)
        {
            try
            {
                var response = await _apiService.PutAsync<UpdateRentalContactDTO, ApiResponse<RentalContactDTO>>("RentalContact", request);
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _apiService.PutAsync<UpdateRentalContactDTO, RentalContactDTO>("RentalContact", request);
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // DELETE /api/RentalContact/{id}
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<ApiResponse<object>>($"RentalContact/{id}");
                return response != null;
            }
            catch
            {
                return false;
            }
        }
    }
}

