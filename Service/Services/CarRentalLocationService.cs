using Service.IServices;
using Repository.IRepositories;
using Service.Common;
using Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Repository.Entities;

namespace Service.Services
{
    public class CarRentalLocationService : ICarRentalLocationService
    {
        private readonly ICarRentalLocationRepository _carRentalLocationRepository;
        private readonly IRentalLocationRepository _rentalLocationRepository;
        private readonly ICarRepository _carRepository;
        private readonly IMapper _mapper;
        public CarRentalLocationService(
            ICarRentalLocationRepository carRentalLocationRepository,
            IRentalLocationRepository rentalLocationRepository,
            ICarRepository carRepository,
            IMapper mapper)
        {
            _carRentalLocationRepository = carRentalLocationRepository;
            _rentalLocationRepository = rentalLocationRepository;
            _carRepository = carRepository;
            _mapper = mapper;
        }
        public async Task<Result<CreateCarRentalLocationDTO>> AddAsync(CreateCarRentalLocationDTO carRentalLocationDTO)
        {
            var car = await _carRepository.GetByIdAsync(carRentalLocationDTO.CarId);
            if (car == null)
            {
                return Result<CreateCarRentalLocationDTO>.Failure("Không tìm thấy xe. Kiểm tra lại Id.");
            }
            var rentalLocation = await _rentalLocationRepository.GetByIdAsync(carRentalLocationDTO.LocationId);
            if (rentalLocation == null)
            {
                return Result<CreateCarRentalLocationDTO>.Failure("Không tìm thấy điểm thuê. Kiểm tra lại Id.");
            }
            var existingAssignment = await _carRentalLocationRepository.GetByCarAndRentalLocationIdAsync(carRentalLocationDTO.CarId, carRentalLocationDTO.LocationId);
            if (existingAssignment != null && existingAssignment.IsDeleted == false)
            {
                return Result<CreateCarRentalLocationDTO>.Failure("Xe đã được gán cho điểm thuê này.");
            }
            else if (existingAssignment != null && existingAssignment.IsDeleted == true)
            {
                existingAssignment.IsDeleted = false;
                existingAssignment.Quantity = carRentalLocationDTO.Quantity;
                await _carRentalLocationRepository.UpdateAsync(existingAssignment);
                return Result<CreateCarRentalLocationDTO>.Success(carRentalLocationDTO, "Gán xe cho điểm thuê thành công.");
            }
            var carRentalLocation = new CarRentalLocation
            {
                Quantity = carRentalLocationDTO.Quantity,
                CarId = car.Id,
                LocationId = rentalLocation.Id,
                Car = car,
                Location = rentalLocation
            };
            await _carRentalLocationRepository.AddAsync(carRentalLocation);
            return Result<CreateCarRentalLocationDTO>.Success(carRentalLocationDTO, "Thêm xe vào địa điểm thuê xe thành công.");
        }
        public async Task<Result<IEnumerable<CarRentalLocationDTO>>> GetAllAsync()
        {
            var carRentalLocations = await _carRentalLocationRepository.GetAllAsync();
            var carRentalLocationDTOs = _mapper.Map<IEnumerable<CarRentalLocationDTO>>(carRentalLocations);
            return Result<IEnumerable<CarRentalLocationDTO>>.Success(carRentalLocationDTOs);
        }
        public async Task<Result<CarRentalLocationDTO>> GetByIdAsync(int id)
        {
            var carRentalLocation = await _carRentalLocationRepository.GetByIdAsync(id);
            if (carRentalLocation == null)
            {
                return Result<CarRentalLocationDTO>.Failure("Không tìm thấy bản ghi. Kiểm tra lại Id.");
            }
            var carRentalLocationDTO = _mapper.Map<CarRentalLocationDTO>(carRentalLocation);
            return Result<CarRentalLocationDTO>.Success(carRentalLocationDTO);
        }
        public async Task<Result<UpdateCarRentalLocationDTO>> UpdateAsync(UpdateCarRentalLocationDTO carRentalLocationDTO)
        {
            var existingCarRentalLocation = await _carRentalLocationRepository.GetByIdAsync(carRentalLocationDTO.Id);
            if (existingCarRentalLocation == null)
            {
                return Result<UpdateCarRentalLocationDTO>.Failure("Không tìm thấy xe tại điểm thuê. Kiểm tra lại Id.");
            }
            existingCarRentalLocation.Quantity = carRentalLocationDTO.Quantity;
            await _carRentalLocationRepository.UpdateAsync(existingCarRentalLocation);
            return Result<UpdateCarRentalLocationDTO>.Success(carRentalLocationDTO, "Cập nhật thành công.");
        }
        public async Task<Result<IEnumerable<CarRentalLocationDTO>>> GetByCarIdAsync(int id)
        {
            var carRentalLocations = await _carRentalLocationRepository.GetByCarIdAsync(id);
            var carRentalLocationDTOs = _mapper.Map<IEnumerable<CarRentalLocationDTO>>(carRentalLocations);
            return Result<IEnumerable<CarRentalLocationDTO>>.Success(carRentalLocationDTOs);
        }
        public async Task<Result<IEnumerable<CarRentalLocationDTO>>> GetByRentalLocationIdAsync(int id)
        {
            var carRentalLocations = await _carRentalLocationRepository.GetByRentalLocationIdAsync(id);
            var carRentalLocationDTOs = _mapper.Map<IEnumerable<CarRentalLocationDTO>>(carRentalLocations);
            return Result<IEnumerable<CarRentalLocationDTO>>.Success(carRentalLocationDTOs);
        }
        public async Task DeleteAsync(int id)
        {
            await _carRentalLocationRepository.DeleteAsync(id);
        }
    }
}
