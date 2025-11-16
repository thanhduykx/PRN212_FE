using AutoMapper;
using Repository.Entities;
using Repository.IRepositories;
using Service.Common;
using Service.DTOs;
using Service.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<Result<IEnumerable<UserDTO>>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<UserDTO>>(users);
            return Result<IEnumerable<UserDTO>>.Success(dtos);
        }

        public async Task<Result<UserDTO>> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return Result<UserDTO>.Failure("Người dùng không tồn tại! Kiểm tra lại Id.");
            }
            var dto = _mapper.Map<UserDTO>(user);
            return Result<UserDTO>.Success(dto);
        }

        public async Task<Result<CreateStaffUserDTO>> AddAsync(CreateStaffUserDTO staffUserDTO)
        {
            var getByEmail = await _userRepository.GetByEmailAsync(staffUserDTO.Email);
            if (getByEmail != null)
            {
                return Result<CreateStaffUserDTO>.Failure("Email đã được sử dụng.");
            }
            var dto = _mapper.Map<User>(staffUserDTO);
            var passwordHash = HashPassword(staffUserDTO.Password);
            var user = new User
            {
                Email = dto.Email,
                Password = dto.Password,
                PasswordHash = passwordHash,
                FullName = dto.FullName,
                Role = "Staff",
                ConfirmEmailToken = null,
                IsEmailConfirmed = true,
                ResetPasswordToken = null,
                ResetPasswordTokenExpiry = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true,
                RentalLocationId = dto.RentalLocationId
            };
            await _userRepository.AddAsync(user);
            return Result<CreateStaffUserDTO>.Success(staffUserDTO, "Tạo tài khoản Staff thành công.");
        }

        public async Task<Result<UpdateUserDTO>> UpdateAsync(UpdateUserDTO updateUserDTO)
        {
            var getUser = await _userRepository.GetByIdAsync(updateUserDTO.UserId);
            if (getUser == null)
            {
                return Result<UpdateUserDTO>.Failure("Người dùng không tồn tại! Kiểm tra lại Id.");
            }
            var passwordHash = HashPassword(updateUserDTO.Password);
            getUser.Email = updateUserDTO.Email;
            getUser.Password = updateUserDTO.Password;
            getUser.PasswordHash = passwordHash;
            getUser.FullName = updateUserDTO.FullName;
            await _userRepository.UpdateAsync(getUser);
            return Result<UpdateUserDTO>.Success(updateUserDTO, "Cập nhật thông tin người dùng thành công.");
        }
        public async Task<Result<UpdateCustomerNameDTO>> UpdateCustomerNameAsync(UpdateCustomerNameDTO updateUserDTO)
        {
            var getUser = await _userRepository.GetByIdAsync(updateUserDTO.UserId);
            if (getUser == null)
            {
                return Result<UpdateCustomerNameDTO>.Failure("Người dùng không tồn tại! Kiểm tra lại Id.");
            }
            if (!getUser.Role.Equals("Customer"))
            {
                return Result<UpdateCustomerNameDTO>.Failure("Chỉ có thể cập nhật tên cho khách hàng.");
            }
            getUser.FullName = updateUserDTO.FullName;
            await _userRepository.UpdateAsync(getUser);
            return Result<UpdateCustomerNameDTO>.Success(updateUserDTO, "Cập nhật tên khách hàng thành công.");
        }
        public async Task<Result<UpdateCustomerPasswordDTO>> UpdateCustomerPasswordAsync(UpdateCustomerPasswordDTO updateUserDTO)
        {
            var getUser = await _userRepository.GetByIdAsync(updateUserDTO.UserId);
            if (getUser == null)
            {
                return Result<UpdateCustomerPasswordDTO>.Failure("Người dùng không tồn tại! Kiểm tra lại Id.");
            }
            if (!getUser.Role.Equals("Customer"))
            {
                return Result<UpdateCustomerPasswordDTO>.Failure("Chỉ có thể cập nhật mật khẩu cho khách hàng.");
            }
            if (!getUser.Password.Equals(updateUserDTO.oldPassword))
            {
                return Result<UpdateCustomerPasswordDTO>.Failure("Mật khẩu cũ không đúng. Vui lòng thử lại.");
            }
            var passwordHash = HashPassword(updateUserDTO.newPassword);
            getUser.Password = updateUserDTO.newPassword;
            getUser.PasswordHash = passwordHash;
            await _userRepository.UpdateAsync(getUser);
            return Result<UpdateCustomerPasswordDTO>.Success(updateUserDTO, "Cập nhật mật khẩu khách hàng thành công.");
        }
        public async Task DeleteAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user != null)
            {
                await _userRepository.DeleteAsync(id);
            }
        }
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
