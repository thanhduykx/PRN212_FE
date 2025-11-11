using AssignmentPRN212.DTO;
using System.Collections.Generic;
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

        public async Task<List<RentalLocationDTO>> GetAllAsync()
        {
            var response = await _apiService.GetAsync<ApiResponse<DataWrapper<RentalLocationDTO>>>("RentalLocation/GetAll");
            return response.Data?.Values ?? new List<RentalLocationDTO>();
        }

        public async Task<List<UserDTO>> GetAllStaffByLocationIdAsync(int locationId)
        {
            var response = await _apiService.GetAsync<ApiResponse<DataWrapper<UserDTO>>>($"RentalLocation/GetAllStaffByLocationId?locationId={locationId}");
            return response.Data?.Values ?? new List<UserDTO>();
        }

        public async Task<RentalLocationDTO?> GetByIdAsync(int id)
        {
            var response = await _apiService.GetAsync<ApiResponse<RentalLocationDTO>>($"RentalLocation/GetById?id={id}");
            return response.Data;
        }

        public async Task<RentalLocationDTO?> CreateAsync(CreateRentalLocationRequest request)
        {
            var response = await _apiService.PostAsync<CreateRentalLocationRequest, ApiResponse<RentalLocationDTO>>("RentalLocation/Create", request);
            return response.Data;
        }

        public async Task<RentalLocationDTO?> UpdateAsync(UpdateRentalLocationRequest request)
        {
            var response = await _apiService.PutAsync<UpdateRentalLocationRequest, ApiResponse<RentalLocationDTO>>("RentalLocation/Update", request);
            return response.Data;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _apiService.DeleteAsync<ApiResponse<object>>($"RentalLocation/{id}");
            return response != null;
        }
    }
}
