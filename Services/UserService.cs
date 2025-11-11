using AssignmentPRN212.DTO;
using AssignmentPRN212.Models;
using System.Collections.Generic;
using System.Net;
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
            _apiService = api;
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
            var response = await _apiService.GetAsync<ApiResponse<DataWrapper<UserDTO>>>("User/GetAll");
            if (response?.Data?.Values != null)
                return response.Data.Values;
            return new List<UserDTO>();
        }

        public async Task<UserDTO?> AddUserAsync(UserDTO user)
        {
            var response = await _apiService.PostAsync<UserDTO, ApiResponse<UserDTO>>("User", user);
            return response.Data;
        }

        public async Task<UserDTO?> UpdateUserAsync(UserDTO user)
        {
            var response = await _apiService.PutAsync<UserDTO, ApiResponse<UserDTO>>($"User/{user.Id}", user);
            return response.Data;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var response = await _apiService.DeleteAsync<ApiResponse<object>>($"User/{id}");
            return response != null;
        }

        public async Task<RegisterResponse?> RegisterAsync(string email, string password, string fullName)
        {
            var request = new RegisterRequest 
            { 
                Email = email, 
                Password = password, 
                FullName = fullName 
            };
            var response = await _apiService.PostAsync<RegisterRequest, RegisterResponse>("Auth/register", request);
            return response;
        }

        public async Task<bool> ConfirmEmailAsync(string token)
        {
            try
            {
                var response = await _apiService.GetStringAsync($"Auth/confirm-email?token={System.Net.WebUtility.UrlEncode(token)}");
                return !string.IsNullOrEmpty(response) && response.Contains("thành công");
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            try
            {
                var request = new ForgotPasswordRequest { Email = email };
                var response = await _apiService.PostAsync<ForgotPasswordRequest, ApiResponse<object>>("Auth/forgot-password", request);
                return response != null;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(string email, string otp, string newPassword)
        {
            try
            {
                var request = new ResetPasswordRequest 
                { 
                    Email = email, 
                    OTP = otp, 
                    NewPassword = newPassword 
                };
                var response = await _apiService.PostAsync<ResetPasswordRequest, ApiResponse<object>>("Auth/reset-password", request);
                return response != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
