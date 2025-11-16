using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.IServices;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAllPayment()
        {
            var Payments = await _paymentService.GetAllAsync();
            return Ok(Payments);
        }

        [HttpGet("GetAllByUserId")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetAllCustomerPayment(int UserId)
        {
            var Payments = await _paymentService.GetAllByUserIdAsync(UserId);
            return Ok(Payments);
        }

        [HttpGet("GetByPaymentId")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetById(int id)
        {
            var payment = await _paymentService.GetByIdAsync(id);
            if (payment == null)
                return NotFound();

            return Ok(payment);
        }

        [HttpPost("Create")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Create([FromBody] CreatePaymentDTO createPaymentDTO)
        {
            if (createPaymentDTO == null)
                return BadRequest("Invalid data.");

            var result = await _paymentService.AddAsync(createPaymentDTO);
            return Ok(result);
        }

        [HttpPut("UpdatePaymentStatus")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Update([FromBody] UpdatePaymentStatusDTO updatePaymentStatusDTO)
        {
            if (updatePaymentStatusDTO == null)
                return BadRequest("Invalid data.");

            var result = await _paymentService.UpdatePaymentStatusAsync(updatePaymentStatusDTO);
            return Ok(result);
        }
    }
}
