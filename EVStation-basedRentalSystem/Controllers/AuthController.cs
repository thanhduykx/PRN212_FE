using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Repository.Entities;
using Repository.Entities.Model;
using Repository.IRepositories;
using Service.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;
        public AuthController(IConfiguration config, IAuthService authService, IUserRepository userRepository)
        {
            _config = config;
            _authService = authService;
            _userRepository = userRepository;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _authService.Authenticate(model.Email, model.Password);

            if (user == null)
                return Unauthorized();

            var token = GenerateJSONWebToken(user);
            return Ok(new
            {
                Token = token,
                UserId = user.Id,
                Role = user.Role,
                FullName = user.FullName
            });
        }

        private string GenerateJSONWebToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"]
                    , _config["Jwt:Audience"]
                    , new Claim[]
                    {
                    new(ClaimTypes.Name, userInfo.FullName),
                    //new(ClaimTypes.Email, userInfo.Email),
                    new(ClaimTypes.Role, userInfo.Role.ToString()),
                    },
                    expires: DateTime.Now.AddMinutes(1200),
                    signingCredentials: credentials
                );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var user = new User
            {
                Email = model.Email,
                Password = model.Password,
                FullName = model.FullName
            };
            await _authService.Register(user, model.Password);

            var response = new
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Message = "Đăng ký thành công!"
            };

            return Ok(response);
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            var user = await _userRepository.GetByConfirmationTokenAsync(token);

            if (user == null)
            {
                return BadRequest("Token không hợp lệ.");
            }

            user.IsEmailConfirmed = true;
            user.ConfirmEmailToken = null;
            user.IsActive = true;

            await _userRepository.UpdateAsync(user);

            return Ok("Email đã được xác nhận thành công.");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
                return BadRequest("Email không được để trống.");

            await _authService.ForgotPasswordAsync(model.Email);

            return Ok("Mã reset đã được gửi. Kiểm tra hộp thư của bạn.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            var success = await _authService.ResetPasswordAsync(model.Email, model.OTP, model.NewPassword);

            if (!success)
                return BadRequest("Mã OTP không hợp lệ hoặc đã hết hạn.");

            return Ok("Mật khẩu đã được reset thành công. Bạn có thể đăng nhập ngay.");
        }
    }

    public class ForgotPasswordModel
    {
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordModel
    {
        public string Email { get; set; } = string.Empty;
        public string OTP { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
