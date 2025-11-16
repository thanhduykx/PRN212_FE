using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.IServices;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CitizenIdController : ControllerBase
    {
        private readonly ICitizenIdService _citizenIdService;
        public CitizenIdController(ICitizenIdService citizenIdService)
        {
            _citizenIdService = citizenIdService;
        }
        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAllCitizenIds()
        {
            var citizenIds = await _citizenIdService.GetAllCitizenIdsAsync();
            return Ok(citizenIds);
        }
        [HttpGet("GetById")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetById(int id)
        {
            var citizenId = await _citizenIdService.GetCitizenIdByIdAsync(id);
            if (citizenId == null)
                return NotFound();
            return Ok(citizenId);
        }
        [HttpGet("GetByOrderId")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetByOrderId(int orderId)
        {
            var citizenId = await _citizenIdService.GetCitizenIdByOrderIdAsync(orderId);
            if (citizenId == null)
                return NotFound();
            return Ok(citizenId);
        }
        [HttpPost("Create")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> Create([FromBody] CreateCitizenIdDTO createCitizenIdDTO)
        {
            if (createCitizenIdDTO == null)
                return BadRequest("Invalid data.");
            var result = await _citizenIdService.CreateCitizenIdAsync(createCitizenIdDTO);
            return Ok(result);
        }
        [HttpPut("UpdateCitizenIdStatus")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateStatus([FromForm] UpdateCitizenIdStatusDTO updateCitizenIdStatusDTO)
        {
            if (updateCitizenIdStatusDTO == null)
                return BadRequest("Invalid data.");
            var result = await _citizenIdService.UpdateCitizenIdStatusAsync(updateCitizenIdStatusDTO);
            return Ok(result);
        }
        [HttpPut("UpdateCitizenIdInfo")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> UpdateInfo([FromBody] UpdateCitizenIdInfoDTO updateCitizenIdInfoDTO)
        {
            if (updateCitizenIdInfoDTO == null)
                return BadRequest("Invalid data.");
            var result = await _citizenIdService.UpdateCitizenIdInfoAsync(updateCitizenIdInfoDTO);
            return Ok(result);
        }
    }
}
