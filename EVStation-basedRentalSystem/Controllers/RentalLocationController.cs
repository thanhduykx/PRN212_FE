using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.IServices;
using Service.Services;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RentalLocationController : ControllerBase
    {
        private readonly IRentalLocationService _rentalLocationService;
        public RentalLocationController(IRentalLocationService rentalLocationService)
        {
            _rentalLocationService = rentalLocationService;
        }
        [HttpGet("GetAll")]
    
        public async Task<IActionResult> GetAllAsync()
        {
            var Users = await _rentalLocationService.GetAllAsync();
            return Ok(Users);
        }
        [HttpGet("GetAllStaffByLocationId")]

        public async Task<IActionResult> GetAllStaffByLocationId(int locationId)
        {
            var Users = await _rentalLocationService.GetAllStaffByLocationIdAsync(locationId);
            return Ok(Users);
        }
        [HttpGet("GetById")]

        public async Task<IActionResult> GetById(int id)
        {
            var user = await _rentalLocationService.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }
        [HttpPost("Create")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Create([FromBody] CreateRentalLocationDTO createRentalLocationDTO)
        {
            if (createRentalLocationDTO == null)
                return BadRequest("Invalid data.");

            var result = await _rentalLocationService.AddAsync(createRentalLocationDTO);
            return Ok(result);
        }

        [HttpPut("Update")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Update([FromBody] UpdateRentalLocationDTO updateRentalLocationDTO)
        {
            if (updateRentalLocationDTO == null)
                return BadRequest("Invalid data.");

            var result = await _rentalLocationService.UpdateAsync(updateRentalLocationDTO);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _rentalLocationService.DeleteAsync(id);
            return Ok("Rental Location deleted successfully.");
        }
    }
}
