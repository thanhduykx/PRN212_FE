using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.IServices;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DriverLicenseController : ControllerBase
    {
        private readonly IDriverLicenseService _driverLicenseService;
        public DriverLicenseController(IDriverLicenseService driverLicenseService)
        {
            _driverLicenseService = driverLicenseService;
        }
        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAllDriverLicenses()
        {
            var driverLicenses = await _driverLicenseService.GetAllAsync();
            return Ok(driverLicenses);
        }
        [HttpGet("GetById")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetById(int id)
        {
            var driverLicense = await _driverLicenseService.GetByIdAsync(id);
            if (driverLicense == null)
                return NotFound();
            return Ok(driverLicense);
        }
        [HttpGet("GetByOrderId")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetByOrderId(int orderId)
        {
            var driverLicense = await _driverLicenseService.GetByOrderIdAsync(orderId);
            if (driverLicense == null)
                return NotFound();
            return Ok(driverLicense);
        }
        [HttpPost("Create")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> Create([FromBody] CreateDriverLicenseDTO createDriverLicenseDTO)
        {
            if (createDriverLicenseDTO == null)
                return BadRequest("Invalid data.");
            var result = await _driverLicenseService.CreateAsync(createDriverLicenseDTO);
            return Ok(result);
        }
        [HttpPut("UpdateDriverLicenseStatus")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateStatus([FromForm] UpdateDriverLicenseStatusDTO updateDriverLicenseStatusDTO)
        {
            if (updateDriverLicenseStatusDTO == null)
                return BadRequest("Invalid data.");
            var result = await _driverLicenseService.UpdateStatusAsync(updateDriverLicenseStatusDTO);
            return Ok(result);
        }
        [HttpPut("UpdateDriverLicenseInfo")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> UpdateInfo([FromForm] UpdateDriverLicenseInfoDTO updateDriverLicenseInfoDTO)
        {
            if (updateDriverLicenseInfoDTO == null)
                return BadRequest("Invalid data.");
            var result = await _driverLicenseService.UpdateInfoAsync(updateDriverLicenseInfoDTO);
            return Ok(result);
        }
    }
}
