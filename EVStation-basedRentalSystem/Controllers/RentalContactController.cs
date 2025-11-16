using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.IServices;
using System.Threading.Tasks;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RentalContactController : ControllerBase
    {
        private readonly IRentalContactService _service;

        public RentalContactController(IRentalContactService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result.Data);
        }

        [HttpGet("byRentalOrder/{rentalOrderId}")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetByRentalOrderId(int rentalOrderId)
        {
            var result = await _service.GetByRentalOrderIdAsync(rentalOrderId);
            if (!result.IsSuccess) return NotFound(result.Message);
            return Ok(result.Data);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Create([FromBody] RentalContactCreateDTO dto)
        {
            var result = await _service.AddAsync(dto);
            if (!result.IsSuccess) return BadRequest(result.Message);
            return Ok(result.Message);
        }

        [HttpPut]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Update([FromBody] RentalContactUpdateDTO dto)
        {
            var result = await _service.UpdateAsync(dto);
            if (!result.IsSuccess) return NotFound(result.Message);
            return Ok(result.Message);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result.IsSuccess) return NotFound(result.Message);
            return Ok(result.Message);
        }
    }
}
