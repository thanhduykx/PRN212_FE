using Microsoft.IdentityModel.Tokens;
using Repository.Entities;
using Repository.IRepositories;
using Service.Common;
using Service.DTOs;
using Service.EmailConfirmation;
using Service.IServices;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly EmailService _emailService;

        public AuthService(IUserRepository userRepository, EmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        public async Task<User> Authenticate(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null || !VerifyPassword(password, user.PasswordHash))
                return null;

            if (!user.IsEmailConfirmed)
                throw new Exception("Vui lòng xác nhận email trước khi đăng nhập.");

            return user;

        }

        public static string GenerateToken(int length = 32)
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] tokenData = new byte[length];
            rng.GetBytes(tokenData);
            return Convert.ToBase64String(tokenData);
        }
        //public static string GenerateEmailToken(int length = 5)
        //{
        //    using var rng = RandomNumberGenerator.Create();
        //    byte[] tokenData = new byte[length];
        //    rng.GetBytes(tokenData);
        //    return Convert.ToBase64String(tokenData);
        //}
        public static string GenerateEmailToken()
        {
            Random random = new Random();
            string token = random.Next(100000, 999999).ToString();
            return token;
        }

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            if (string.IsNullOrWhiteSpace(user.Role))
                throw new ArgumentException("User role is required");


            var key = Encoding.ASCII.GetBytes(GenerateToken());
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                     new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                     new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(120),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<Result<AuthDTO>> Register(User user, string password)
        {
            if (user == null)
            {
                var msg = new AuthDTO { Message = "Người dùng không được để trống." };
                return Result<AuthDTO>.Success(msg);
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                var msg = new AuthDTO { Message = "Mật khẩu không được để trống." };
                return Result<AuthDTO>.Success(msg);
            }

            var existingUser = await _userRepository.GetByEmailAsync(user.Email);
            if (existingUser != null)
            {
                var msg = new AuthDTO { Message = "Email đã được sử dụng." };
                return Result<AuthDTO>.Success(msg);
            }

            user.Role = string.IsNullOrWhiteSpace(user.Role) ? "Customer" : user.Role;

            user.PasswordHash = HashPassword(password);

            user.ConfirmEmailToken = GenerateEmailToken();
            user.IsEmailConfirmed = false;

            using (var transaction = await _userRepository.BeginTransactionAsync())
            {
                try
                {
                    await _emailService.SendConfirmationEmail(user.Email, user.ConfirmEmailToken);

                    await _userRepository.AddAsync(user);

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    Console.WriteLine($"Lỗi khi đăng ký người dùng: {ex.Message}");

                    throw;
                }
            }
            var successMsg = new AuthDTO { Message = "Đăng ký thành công! Vui lòng kiểm tra email để xác nhận." };
            return Result<AuthDTO>.Success(successMsg);
        }
        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || !user.IsActive)
            {
                return;
            }

            string otp = GenerateEmailToken();
            user.ResetPasswordToken = otp;
            user.ResetPasswordTokenExpiry = DateTime.UtcNow.AddMinutes(15);
            await _emailService.SendResetPasswordEmailAsync(user.Email, user.FullName, otp);

            await _userRepository.UpdateAsync(user);
        }

        public async Task<bool> ResetPasswordAsync(string email, string otp, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
                throw new ArgumentException("Mật khẩu mới phải ít nhất 8 ký tự.");

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || !user.IsActive)
                return false;

            // Validate OTP và expiry
            if (user.ResetPasswordToken != otp || user.ResetPasswordTokenExpiry < DateTime.UtcNow)
                return false;

            // Update password hash
            user.PasswordHash = HashPassword(newPassword);
            user.Password = newPassword;
            user.ResetPasswordToken = null;
            user.ResetPasswordTokenExpiry = null;
            user.UpdatedAt = DateTime.UtcNow;
            user.IsEmailConfirmed = true;

            await _userRepository.UpdateAsync(user);
            return true;
        }
    }
}
