using AssignmentPRN212.DTO;
using AssignmentPRN212.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssignmentPRN212.Services
{
    public class UserService
    {
        private readonly ApiService _api;

        public UserService(ApiService api)
        {
            _api = api;
        }

        // Login
        public async Task<LoginResponse> LoginAsync(string email, string password)
        {
            var request = new LoginRequest { Email = email, Password = password };
            var response = await _api.PostAsync<LoginRequest, LoginResponse>("Auth/login", request);

            if (!string.IsNullOrEmpty(response.Token))
                _api.SetToken(response.Token); // lưu token để gọi API khác

            return response;
        }

        // Lấy danh sách tất cả user
        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            var response = await _api.GetAsync<ApiResponse<DataWrapper<UserDTO>>>("User/GetAll");
            return response?.Data?.Values ?? new List<UserDTO>();
        }

        // Lấy user theo Id
        public async Task<UserDTO?> GetUserByIdAsync(int id)
        {
            var response = await _api.GetAsync<ApiResponse<UserDTO>>($"User/GetById?id={id}");
            return response?.Data;
        }

        // Thêm staff
        public async Task<UserDTO?> AddStaffAsync(CreateStaffUserDTO staff)
        {
            var response = await _api.PostAsync<CreateStaffUserDTO, ApiResponse<UserDTO>>("User/CreateStaff", staff);
            return response?.Data;
        }

        // Cập nhật user
        public async Task<UserDTO?> UpdateUserAsync(UpdateUserDTO user)
        {
            var response = await _api.PutAsync<UpdateUserDTO, ApiResponse<UserDTO>>("User", user);
            return response?.Data;
        }

        // Cập nhật tên customer
        public async Task<UserDTO?> UpdateCustomerNameAsync(UpdateCustomerNameDTO dto)
        {
            try
            {
                var response = await _api.PutAsync<UpdateCustomerNameDTO, ApiResponse<UserDTO>>("User/UpdateCustomerName", dto);
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _api.PutAsync<UpdateCustomerNameDTO, UserDTO>("User/UpdateCustomerName", dto);
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // Cập nhật password customer
        public async Task<UserDTO?> UpdateCustomerPasswordAsync(UpdateCustomerPasswordDTO dto)
        {
            try
            {
                var response = await _api.PutAsync<UpdateCustomerPasswordDTO, ApiResponse<UserDTO>>("User/UpdateCustomerPassword", dto);
                return response?.Data;
            }
            catch
            {
                try
                {
                    var directResponse = await _api.PutAsync<UpdateCustomerPasswordDTO, UserDTO>("User/UpdateCustomerPassword", dto);
                    return directResponse;
                }
                catch
                {
                    return null;
                }
            }
        }

        // Xóa user
        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                var response = await _api.DeleteAsync<ApiResponse<object>>($"User/{id}");
                return response != null;
            }
            catch
            {
                return false;
            }
        }

        // Register
        public async Task<RegisterResponse?> RegisterAsync(string email, string password, string fullName)
        {
            var request = new RegisterRequest { Email = email, Password = password, FullName = fullName };
            var response = await _api.PostAsync<RegisterRequest, RegisterResponse>("Auth/register", request);
            return response;
        }

        // Confirm email
        public async Task<bool> ConfirmEmailAsync(string token)
        {
            try
            {
                var response = await _api.GetStringAsync($"Auth/confirm-email?token={Uri.EscapeDataString(token)}");
                return !string.IsNullOrEmpty(response) && response.Contains("thành công");
            }
            catch
            {
                return false;
            }
        }

        // Forgot password
        public async Task<bool> ForgotPasswordAsync(string email)
        {
            try
            {
                var request = new ForgotPasswordRequest { Email = email };
                var response = await _api.PostAsync<ForgotPasswordRequest, ApiResponse<object>>("Auth/forgot-password", request);
                return response != null;
            }
            catch
            {
                return false;
            }
        }

        // Reset password
        public async Task<bool> ResetPasswordAsync(string email, string otp, string newPassword)
        {
            try
            {
                var request = new ResetPasswordRequest { Email = email, OTP = otp, NewPassword = newPassword };
                var response = await _api.PostAsync<ResetPasswordRequest, ApiResponse<object>>("Auth/reset-password", request);
                return response != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
