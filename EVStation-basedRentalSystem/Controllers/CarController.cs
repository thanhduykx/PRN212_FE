using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Service.IServices;
using System;
using System.Threading.Tasks;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CarController : ControllerBase
    {
        private readonly ICarService _carService;

        public CarController(ICarService carService)
        {
            _carService = carService;
        }

        // ✅ GET: api/Car
        [HttpGet]
       
        public async Task<IActionResult> GetAll()
        {
            var result = await _carService.GetAllAsync();
            if (!result.IsSuccess)
                return NotFound(new { message = result.Message });

            return Ok(new { message = result.Message, data = result.Data });
        }

        // ✅ GET: api/Car/byName/{name}
        [HttpGet("byName/{name}")]
      
        public async Task<IActionResult> GetByName(string name)
        {
            var result = await _carService.GetByNameAsync(name);
            if (!result.IsSuccess)
                return NotFound(new { message = result.Message });

            return Ok(new { message = result.Message, data = result.Data });
        }

        // ✅ GET: api/Car/paged?pageIndex=0&pageSize=10&keyword=...
        [HttpGet("paged")]
   
        public async Task<IActionResult> GetPaged(
            [FromQuery] int pageIndex = 0,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? keyword = null)
        {
            var result = await _carService.GetPagedAsync(pageIndex, pageSize, keyword);
            if (!result.IsSuccess)
                return NotFound(new { message = result.Message });

            return Ok(new
            {
                message = result.Message,
                total = result.Data.Total,
                data = result.Data.Data
            });
        }

        // ✅ POST: api/Car
        [Authorize(Roles = "Staff,Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Car car)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

            car.CreatedAt = vietnamTime;
            car.UpdatedAt = vietnamTime;

            var result = await _carService.AddAsync(car);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });

            return CreatedAtAction(nameof(GetByName), new { name = car.Name }, new
            {
                message = result.Message,
                data = result.Data
            });
        }

        // ✅ PUT: api/Car/{id}
        [Authorize(Roles = "Staff,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Car car)
        {
            if (id != car.Id)
                return BadRequest(new { message = "ID không khớp." });

            var vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            car.UpdatedAt = vietnamTime;

            var result = await _carService.UpdateAsync(car);
            if (!result.IsSuccess)
                return NotFound(new { message = result.Message });

            return Ok(new { message = result.Message, data = result.Data });
        }

        // ✅ DELETE: api/Car/{id}
        [Authorize(Roles = "Staff,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _carService.DeleteAsync(id);
            if (!result.IsSuccess)
                return NotFound(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        // ✅ GET: api/Car/TopRented?topCount=3
        [HttpGet("TopRented")]
    
        public async Task<IActionResult> GetTopRentedCars([FromQuery] int topCount = 3)
        {
            var result = await _carService.GetTopRentedAsync(topCount);
            if (!result.IsSuccess)
                return NotFound(new { message = result.Message });

            return Ok(new { message = result.Message, data = result.Data });
        }
    }
}
