using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Service.IServices;
using System.Threading.Tasks;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        // 🔹 Lấy tất cả feedback (không phân trang)
        [HttpGet]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _feedbackService.GetAllAsync();
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        // 🔹 GET: api/Feedback/paged?pageIndex=1&pageSize=5&keyword=abc
        [HttpGet("paged")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetPaged([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 5, [FromQuery] string? keyword = null)
        {
            var result = await _feedbackService.GetPagedAsync(pageIndex, pageSize, keyword);
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(new { total = result.Data.Total, data = result.Data.Data });
        }

        // 🔹 GET: api/Feedback/byCar/Toyota
        [HttpGet("byCar/{carName}")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetByCarName(string carName)
        {
            var result = await _feedbackService.GetByCarNameAsync(carName);
            if (!result.IsSuccess)
                return NotFound(result.Message);

            return Ok(result.Data);
        }

        // 🔹 POST: api/Feedback
        [HttpPost]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> Create([FromBody] Feedback fb)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst("Id");
            if (userIdClaim == null)
                return Unauthorized("Không thể xác định người dùng.");

            fb.UserId = int.Parse(userIdClaim.Value);
            fb.CreatedAt = DateTime.UtcNow;
            fb.IsDeleted = false;

            var result = await _feedbackService.AddAsync(fb);
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(new { Message = result.Message, fb });
        }

        // 🔹 PUT: api/Feedback/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> Update(int id, [FromBody] Feedback fb)
        {
            var userIdClaim = User.FindFirst("Id");
            if (userIdClaim == null)
                return Unauthorized("Không thể xác định người dùng.");

            var allFeedbacks = await _feedbackService.GetAllAsync();
            var existing = allFeedbacks.Data.FirstOrDefault(f => f.Id == id && !f.IsDeleted);

            if (existing == null)
                return NotFound("Feedback không tồn tại.");

            if (existing.UserId != int.Parse(userIdClaim.Value))
                return Forbid("Bạn không có quyền chỉnh sửa feedback này.");

            existing.Title = fb.Title;
            existing.Content = fb.Content;
            existing.UpdatedAt = DateTime.UtcNow;

            var result = await _feedbackService.UpdateAsync(existing);
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(new { Message = result.Message, existing });
        }

        // 🔹 DELETE: api/Feedback/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> Delete(int id)
        {
            var userIdClaim = User.FindFirst("Id");
            if (userIdClaim == null)
                return Unauthorized("Không thể xác định người dùng.");

            var allFeedbacks = await _feedbackService.GetAllAsync();
            var feedback = allFeedbacks.Data.FirstOrDefault(f => f.Id == id && !f.IsDeleted);

            if (feedback == null)
                return NotFound("Feedback không tồn tại.");

            if (feedback.UserId != int.Parse(userIdClaim.Value))
                return Forbid("Bạn không có quyền xóa feedback này.");

            var result = await _feedbackService.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(new { Message = result.Message });
        }
    }
}
