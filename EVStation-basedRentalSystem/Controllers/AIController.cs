using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.Context;
using Repository.Entities;
using Service.IServices;

namespace EVStation_basedRentalSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIController : ControllerBase
    {
        private readonly IAIService _aiService;
        private readonly EVSDbContext _dbContext;
        private readonly ILogger<AIController> _logger;

        public AIController(IAIService aiService, EVSDbContext dbContext, ILogger<AIController> logger)
        {
            _aiService = aiService;
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Phân tích dữ liệu từ database và trả về gợi ý nâng cấp
        /// </summary>
        [HttpGet("analyze")]
        public async Task<IActionResult> AnalyzeData()
        {
            try
            {
                var cars = await _dbContext.Cars
                    .Where(c => !c.IsDeleted && c.IsActive)
                    .Take(5)
                    .Select(c => new { c.Name, c.Model, c.Seats, c.BatteryDuration, c.RentPricePerDay })
                    .ToListAsync();

                var feedbacks = await _dbContext.Feedbacks
                    .Where(f => !f.IsDeleted)
                    .OrderByDescending(f => f.CreatedAt)
                    .Take(5)
                    .Select(f => new { f.Title, f.Content })
                    .ToListAsync();

                var rentalOrders = await _dbContext.RentalOrders
                    .OrderByDescending(r => r.OrderDate)
                    .Take(5)
                    .Select(r => new { r.Id, r.WithDriver, r.Status, r.Total })
                    .ToListAsync();

                var prompt = @$"
Bạn là chuyên gia phân tích dữ liệu thuê xe.
Dưới đây là thông tin các xe, đơn hàng và phản hồi khách hàng:

Xe: {System.Text.Json.JsonSerializer.Serialize(cars)}
Phản hồi khách hàng: {System.Text.Json.JsonSerializer.Serialize(feedbacks)}
Đơn hàng: {System.Text.Json.JsonSerializer.Serialize(rentalOrders)}

Hãy phân tích và đưa ra gợi ý nâng cấp dịch vụ hoặc cải thiện xe.
Trả lời ngắn, tối đa 1000 ký tự.
Dễ nhìn
- Không dùng dấu *, ** hay markdown.
- Dùng các dòng tách nhau bằng xuống dòng \n.
- Mỗi ý là 1 câu dễ đọc, thân thiện với người xem.
";

                var aiResponse = await _aiService.GenerateResponseAsync(prompt);

                return Ok(new { response = aiResponse });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing data for AI");
                return StatusCode(500, new { error = ex.Message });
            }
        }
        /// <summary>
        /// Phân tích tỷ lệ sử dụng xe và giờ cao điểm
        /// </summary>
        [HttpGet("car-usage")]
        public async Task<IActionResult> AnalyzeCarUsage()
        {
            try
            {
                // Lấy dữ liệu Car, CarDeliveryHistory và RentalOrder
                var cars = await _dbContext.Cars
                    .Where(c => !c.IsDeleted && c.IsActive)
                    .Select(c => new { c.Id, c.Name })
                    .ToListAsync();

                var deliveries = await _dbContext.CarDeliveryHistories
                    .Include(d => d.Order)
                    .ToListAsync();

                var rentalOrders = await _dbContext.RentalOrders
                    .Where(r => r.Status.ToString() == "Completed" || r.Status.ToString() == "Ongoing")
                    .ToListAsync();

                // Tính tỷ lệ sử dụng xe: số đơn trên tổng xe
                var usageData = cars.Select(c =>
                {
                    var count = rentalOrders.Count(r => r.CarId == c.Id);
                    var ratio = cars.Count > 0 ? (double)count / cars.Count : 0;
                    return new { c.Name, Rentals = count, UsageRatio = ratio };
                }).ToList();

                // Lấy giờ cao điểm dựa trên PickupTime hoặc DeliveryDate
                var peakHours = rentalOrders
                    .Select(r => r.PickupTime.Hour)
                    .GroupBy(h => h)
                    .Select(g => new { Hour = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(3) // 3 giờ cao điểm nhiều nhất
                    .ToList();

                // Tạo prompt gửi AI
                var prompt = @$"
Bạn là chuyên gia phân tích dữ liệu thuê xe.
Dựa trên dữ liệu dưới đây:

Danh sách xe và số lần thuê: {System.Text.Json.JsonSerializer.Serialize(usageData)}
Giờ cao điểm thuê xe: {System.Text.Json.JsonSerializer.Serialize(peakHours)}

Hãy phân tích tỷ lệ sử dụng xe và xác định giờ cao điểm.
Trả lời ngắn cỡ 1000 từ, dễ đọc, mỗi ý 1 dòng.
Không dùng *, ** hay markdown.
";

                var aiResponse = await _aiService.GenerateResponseAsync(prompt);

                return Ok(new { response = aiResponse });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing car usage");
                return StatusCode(500, new { error = ex.Message });
            }
        }

    }
}
