using AssignmentPRN212.DTO;
using AssignmentPRN212.Models;
using System.Threading.Tasks;

namespace AssignmentPRN212.Services
{
    public class UserService
    {
        private readonly ApiService _api;
        private readonly ApiService _apiService;
        public UserService(ApiService api)
        {
            _api = api;
        }

        public async Task<LoginResponse> LoginAsync(string email, string password)
        {
            var request = new LoginRequest { Email = email, Password = password };
            var response = await _api.PostAsync<LoginRequest, LoginResponse>("Auth/login", request);

            if (!string.IsNullOrEmpty(response.Token))
                _api.SetToken(response.Token); // lưu token để gọi API khác

            return response;
        }
        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            var response = await _apiService.GetAsync<ApiResponse<DataWrapper<UserDTO>>>("User");
            if (response?.Data?.Values != null)
                return response.Data.Values;
            return new List<UserDTO>();
        }
    }
}
