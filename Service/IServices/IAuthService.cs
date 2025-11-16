using Repository.Entities;
using Service.Common;
using Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface IAuthService
    {
        string GenerateJwtToken(User user);
        Task<User> Authenticate(string email, string password);
        Task<Result<AuthDTO>> Register(User user, string password);
        Task ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string otp, string newPassword);
    }
}
