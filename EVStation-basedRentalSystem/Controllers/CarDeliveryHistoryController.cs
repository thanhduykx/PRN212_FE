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
    public class CarDeliveryHistoryController : ControllerBase
    {
        private readonly ICarDeliveryHistoryService _service;

        public CarDeliveryHistoryController(ICarDeliveryHistoryService service)
        {
            _service = service;
        }

        // 📘 GET: api/CarDeliveryHistory?pageIndex=1&pageSize=10
        [HttpGet]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetAllAsync(pageIndex, pageSize);
            return Ok(new { total = result.Data.Total, data = result.Data.Data });

        }

        // 📘 GET: api/CarDeliveryHistory/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(new { result.Message });

            return Ok(result);
        }

        // 📗 POST: api/CarDeliveryHistory
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Create([FromBody] CarDeliveryHistoryCreateDTO dto)
        {
            var result = await _service.AddAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(new { result.Message });

            return Ok(new { result.Message });
        }

        // 📙 PUT: api/CarDeliveryHistory/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Update(int id, [FromBody] CarDeliveryHistoryUpdateDTO dto)
        {
            dto.Id = id; // gán id từ route cho DTO
            var result = await _service.UpdateAsync(id, dto);
            if (!result.IsSuccess)
                return NotFound(new { result.Message });

            return Ok(new { result.Message });
        }


        // 📕 DELETE: api/CarDeliveryHistory/{id}
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
