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
    public class CarReturnHistoryController : ControllerBase
    {
        private readonly ICarReturnHistoryService _service;

        public CarReturnHistoryController(ICarReturnHistoryService service)
        {
            _service = service;
        }

        // 📘 GET: api/CarReturnHistory
        [HttpGet]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            if (!result.IsSuccess)
                return BadRequest(new { result.Message });

            return Ok(new
            {
                message = result.Message,
                data = result.Data
            });
        }

        // 📘 GET: api/CarReturnHistory/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(new { result.Message });

            return Ok(new
            {
                message = result.Message,
                data = result.Data
            });
        }

        // 📗 POST: api/CarReturnHistory
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Create([FromBody] CarReturnHistoryCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.AddAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(new { result.Message });

            return Ok(new { result.Message });
        }

        // 📙 PUT: api/CarReturnHistory/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Update(int id, [FromBody] CarReturnHistoryCreateDTO dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            if (!result.IsSuccess)
                return NotFound(new { result.Message });

            return Ok(new { result.Message });
        }

        // 📕 DELETE: api/CarReturnHistory/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result.IsSuccess)
                return NotFound(new { result.Message });

            return Ok(new { result.Message });
        }
    }
}
