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
    public class RentalLocationService : IRentalLocationService
    {
        private readonly IRentalLocationRepository _rentalLocationRepository;
        private readonly IMapper _mapper;
        public RentalLocationService(IRentalLocationRepository rentalLocationRepository, IMapper mapper)
        {
            _rentalLocationRepository = rentalLocationRepository;
            _mapper = mapper;
        }

        public async Task<Result<CreateRentalLocationDTO>> AddAsync(CreateRentalLocationDTO rentalLocationDTO)
        {
            var dto = _mapper.Map<RentalLocation>(rentalLocationDTO);
            var rentalLocation = new RentalLocation
            {
                Name = dto.Name,
                Address = dto.Address,
                Coordinates = dto.Coordinates,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };
            await _rentalLocationRepository.AddAsync(rentalLocation);
            return Result<CreateRentalLocationDTO>.Success(rentalLocationDTO, "Tạo địa điểm thuê xe thành công.");
        }

        public async Task DeleteAsync(int id)
        {
            var rentalLocation = await _rentalLocationRepository.GetByIdAsync(id);
            if (rentalLocation != null)
            {
                rentalLocation.IsDeleted = true;
                await _rentalLocationRepository.UpdateAsync(rentalLocation);
            }
        }

        public async Task<Result<IEnumerable<RentalLocationDTO>>> GetAllAsync()
        {
            var rentalLocations = await _rentalLocationRepository.GetAllAsync();
            var rentalLocationDTOs = _mapper.Map<IEnumerable<RentalLocationDTO>>(rentalLocations);
            return Result<IEnumerable<RentalLocationDTO>>.Success(rentalLocationDTOs);
        }
        public async Task<Result<IEnumerable<UserDTO>>> GetAllStaffByLocationIdAsync(int id)
        {
            var staffMembers = await _rentalLocationRepository.GetAllStaffByLocationAsync(id);
            var staffDTOs = _mapper.Map<IEnumerable<UserDTO>>(staffMembers);
            return Result<IEnumerable<UserDTO>>.Success(staffDTOs);
        }
        public async Task<Result<RentalLocationDTO>> GetByIdAsync(int id)
        {
            var rentalLocation = await _rentalLocationRepository.GetByIdAsync(id);
            if (rentalLocation == null)
            {
                return Result<RentalLocationDTO>.Failure("Địa điểm thuê xe không tồn tại. Kiểm tra lại Id.");
            }
            var rentalLocationDTO = _mapper.Map<RentalLocationDTO>(rentalLocation);
            return Result<RentalLocationDTO>.Success(rentalLocationDTO);
        }

        public async Task<Result<UpdateRentalLocationDTO>> UpdateAsync(UpdateRentalLocationDTO rentalLocationDTO)
        {
            var existingRentalLocation = await _rentalLocationRepository.GetByIdAsync(rentalLocationDTO.Id);
            if (existingRentalLocation == null)
            {
                return Result<UpdateRentalLocationDTO>.Failure("Địa điểm thuê xe không tồn tại. Kiểm tra lại Id.");
            }
            existingRentalLocation.Name = rentalLocationDTO.Name;
            existingRentalLocation.Address = rentalLocationDTO.Address;
            existingRentalLocation.Coordinates = rentalLocationDTO.Coordinates;
            existingRentalLocation.IsActive = rentalLocationDTO.IsActive;
            existingRentalLocation.UpdatedAt = DateTime.UtcNow;
            await _rentalLocationRepository.UpdateAsync(existingRentalLocation);
            return Result<UpdateRentalLocationDTO>.Success(rentalLocationDTO, "Cập nhật địa điểm thuê xe thành công.");
        }
    }
}
