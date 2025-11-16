using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Service.DTOs;
using Service.IServices;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAllUser()
        {
            var Users = await _userService.GetAllAsync();
            return Ok(Users);
        }
        [HttpGet("GetById")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }
        [HttpPost("CreateStaff")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaffUserDTO staffUserDTO)
        {
            if (staffUserDTO == null)
                return BadRequest("Invalid data.");

            var result = await _userService.AddAsync(staffUserDTO);
            return Ok(result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Update([FromBody] UpdateUserDTO updateUserDTO)
        {
            if (updateUserDTO == null)
                return BadRequest("Invalid data.");

            var result = await _userService.UpdateAsync(updateUserDTO);
            return Ok(result);
        }

        [HttpPut("UpdateCustomerName")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> UpdateCustomerName([FromBody] UpdateCustomerNameDTO updateUserDTO)
        {
            if (updateUserDTO == null)
                return BadRequest("Invalid data.");

            var result = await _userService.UpdateCustomerNameAsync(updateUserDTO);
            return Ok(result);
        }

        [HttpPut("UpdateCustomerPassword")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> UpdateCustomerPassword([FromBody] UpdateCustomerPasswordDTO updateUserDTO)
        {
            if (updateUserDTO == null)
                return BadRequest("Invalid data.");

            var result = await _userService.UpdateCustomerPasswordAsync(updateUserDTO);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteAsync(id);
            return Ok("User deleted successfully.");
        }
    }
}
