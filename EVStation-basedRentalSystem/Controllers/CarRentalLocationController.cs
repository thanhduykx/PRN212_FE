using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.IServices;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CarRentalLocationController : ControllerBase
    {
        private readonly ICarRentalLocationService _carRentalLocationService;
        public CarRentalLocationController(ICarRentalLocationService carRentalLocationService)
        {
            _carRentalLocationService = carRentalLocationService;
        }
        [HttpGet("GetAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAsync()
        {
            var carRentalLocations = await _carRentalLocationService.GetAllAsync();
            return Ok(carRentalLocations);
        }
        [HttpGet("GetById")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetById(int id)
        {
            var carRentalLocation = await _carRentalLocationService.GetByIdAsync(id);
            if (carRentalLocation == null)
                return NotFound();
            return Ok(carRentalLocation);
        }
        [HttpGet("GetByCarId")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetByCarId(int carId)
        {
            var carRentalLocations = await _carRentalLocationService.GetByCarIdAsync(carId);
            return Ok(carRentalLocations);
        }
        [HttpGet("GetByRentalLocationId")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetByLocationId(int locationId)
        {
            var carRentalLocations = await _carRentalLocationService.GetByRentalLocationIdAsync(locationId);
            return Ok(carRentalLocations);
        }
        [HttpPost("Create")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Create([FromBody] CreateCarRentalLocationDTO carRentalLocationDTO)
        {
            if (carRentalLocationDTO == null)
                return BadRequest("Invalid data.");
            var result = await _carRentalLocationService.AddAsync(carRentalLocationDTO);
            return Ok(result);
        }
        [HttpPut("Update")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Update([FromBody] UpdateCarRentalLocationDTO carRentalLocationDTO)
        {
            if (carRentalLocationDTO == null)
                return BadRequest("Invalid data.");
            var result = await _carRentalLocationService.UpdateAsync(carRentalLocationDTO);
            return Ok(result);
        }
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Delete(int id)
        {
            await _carRentalLocationService.DeleteAsync(id);
            return Ok("Car Rental Location deleted successfully.");
        }
    }
}
