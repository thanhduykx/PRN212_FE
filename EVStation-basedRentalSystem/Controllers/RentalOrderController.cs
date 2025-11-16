using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.IServices;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RentalOrderController : ControllerBase
    {
        private readonly IRentalOrderService _rentalOrderService;
        public RentalOrderController(IRentalOrderService rentalOrderService)
        {
            _rentalOrderService = rentalOrderService;
        }
        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _rentalOrderService.GetAllAsync();
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpGet("GetById")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _rentalOrderService.GetByIdAsync(id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpGet("GetByUserId")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var result = await _rentalOrderService.GetByUserIdAsync(userId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPost("Create")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> Create([FromBody] CreateRentalOrderDTO createRentalOrderDTO)
        {
            var result = await _rentalOrderService.CreateAsync(createRentalOrderDTO);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPut("UpdateStatus")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateStatus([FromForm] UpdateRentalOrderStatusDTO updateRentalOrderStatusDTO)
        {
            var result = await _rentalOrderService.UpdateStatusAsync(updateRentalOrderStatusDTO);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPut("UpdateTotal")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateTotal([FromBody] UpdateRentalOrderTotalDTO updateRentalOrderTotalDTO)
        {
            var result = await _rentalOrderService.UpdateTotalAsync(updateRentalOrderTotalDTO);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
