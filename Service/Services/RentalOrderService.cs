using AutoMapper;
using Repository.Entities;
using Repository.Entities.Enum;
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
    public class RentalOrderService : IRentalOrderService
    {
        private readonly IRentalOrderRepository _rentalOrderRepository;
        private readonly ICitizenIdRepository _citizenIdRepository;
        private readonly IDriverLicenseRepository _driverLicenseRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICarRepository _carRepository;
        private readonly ICarRentalLocationRepository _carRentalLocationRepository;
        private readonly IRentalLocationRepository _rentalLocationRepository;
        private readonly IRentalContactRepository _rentalContactRepository;
        private readonly IMapper _mapper;
        public RentalOrderService(
            IRentalOrderRepository rentalOrderRepository,
            ICitizenIdRepository citizenIdRepository,
            IDriverLicenseRepository driverLicenseRepository,
            IUserRepository userRepository,
            ICarRepository carRepository,
            IRentalContactRepository rentalContactRepository,
            IMapper mapper,
            ICarRentalLocationRepository carRentalLocationRepository,
            IRentalLocationRepository rentalLocationRepository)
        {
            _rentalOrderRepository = rentalOrderRepository;
            _citizenIdRepository = citizenIdRepository;
            _driverLicenseRepository = driverLicenseRepository;
            _userRepository = userRepository;
            _carRepository = carRepository;
            _rentalContactRepository = rentalContactRepository;
            _mapper = mapper;
            _carRentalLocationRepository = carRentalLocationRepository;
            _rentalLocationRepository = rentalLocationRepository;
        }
        public async Task<Result<IEnumerable<RentalOrderDTO>>> GetAllAsync()
        {
            var rentalOrders = await _rentalOrderRepository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<RentalOrderDTO>>(rentalOrders);
            return Result<IEnumerable<RentalOrderDTO>>.Success(dtos);
        }
        public async Task<Result<RentalOrderDTO>> GetByIdAsync(int id)
        {
            var rentalOrder = await _rentalOrderRepository.GetByIdAsync(id);
            if (rentalOrder == null)
            {
                return Result<RentalOrderDTO>.Failure("Đơn đặt thuê không tồn tại! Kiểm tra lại Id.");
            }
            var dto = _mapper.Map<RentalOrderDTO>(rentalOrder);
            return Result<RentalOrderDTO>.Success(dto);
        }
        public async Task<Result<CreateRentalOrderDTO>> CreateAsync(CreateRentalOrderDTO createRentalOrderDTO)
        {
            var dto = _mapper.Map<RentalOrder>(createRentalOrderDTO);
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null)
            {
                return Result<CreateRentalOrderDTO>.Failure("Người dùng không tồn tại! Kiểm tra lại Id của người dùng.");
            }
            var car = await _carRepository.GetByIdAsync(dto.CarId);
            if (car == null)
            {
                return Result<CreateRentalOrderDTO>.Failure("Xe không tồn tại! Kiểm tra lại Id của xe.");
            }
            var location = await _rentalLocationRepository.GetByIdAsync(dto.RentalLocationId);
            if (location == null)
            {
                return Result<CreateRentalOrderDTO>.Failure("Địa điểm thuê xe không tồn tại! Kiểm tra lại Id của địa điểm.");
            }
            var carRentalLocation = await _carRentalLocationRepository.GetByCarAndLocationIdAsync(dto.CarId, dto.RentalLocationId);
            if (carRentalLocation == null || carRentalLocation.Quantity == 0)
            {
                return Result<CreateRentalOrderDTO>.Failure("Xe không có sẵn tại địa điểm thuê xe đã chọn.");
            }
            var subtotalDays = (dto.ExpectedReturnTime - dto.PickupTime).TotalDays;
            var subtotal = dto.WithDriver
                            ? subtotalDays * car.RentPricePerDayWithDriver
                            : subtotalDays * car.RentPricePerDay;
            //if (dto.WithDriver)
            //{
            //    subtotal = (int)(subtotalDays * car.RentPricePerDayWithDriver);
            //} else
            //{
            //    subtotal = (int)(subtotalDays * car.RentPricePerDay);
            //}
            var order = new RentalOrder
            {
                PhoneNumber = dto.PhoneNumber,
                PickupTime = dto.PickupTime,
                ExpectedReturnTime = dto.ExpectedReturnTime,
                WithDriver = dto.WithDriver,
                SubTotal = subtotal,
                UserId = user.Id,
                User = user,
                CarId = car.Id,
                Car = car,
                RentalLocationId = dto.RentalLocationId,
                RentalLocation = location,
                CreatedAt = DateTime.Now,
                Status = RentalOrderStatus.Pending
            };
            await _rentalOrderRepository.AddAsync(order);
            return Result<CreateRentalOrderDTO>.Success(createRentalOrderDTO, "Tạo Order thành công!");
        }
        public async Task<Result<UpdateRentalOrderTotalDTO>> UpdateTotalAsync(UpdateRentalOrderTotalDTO updateRentalOrderTotalDTO)
        {
            var existingOrder = await _rentalOrderRepository.GetByIdAsync(updateRentalOrderTotalDTO.OrderId);
            if (existingOrder == null)
            {
                return Result<UpdateRentalOrderTotalDTO>.Failure("Đơn đặt thuê không tồn tại! Kiểm tra lại Id.");
            }
            var total = (existingOrder.SubTotal ?? 0) + updateRentalOrderTotalDTO.ExtraFee + updateRentalOrderTotalDTO.DamageFee;
            existingOrder.Total = total;
            existingOrder.ExtraFee = updateRentalOrderTotalDTO.ExtraFee;
            existingOrder.DamageFee = updateRentalOrderTotalDTO.DamageFee;
            existingOrder.DamageNotes = updateRentalOrderTotalDTO.DamageNotes;
            existingOrder.UpdatedAt = DateTime.Now;
            await _rentalOrderRepository.UpdateAsync(existingOrder);
            return Result<UpdateRentalOrderTotalDTO>.Success(updateRentalOrderTotalDTO, "Cập nhật tổng tiền cho đơn hàng thành công!");
        }
        public async Task<Result<UpdateRentalOrderStatusDTO>> UpdateStatusAsync(UpdateRentalOrderStatusDTO updateRentalOrderStatusDTO)
        {
            var existingOrder = await _rentalOrderRepository.GetByIdAsync(updateRentalOrderStatusDTO.OrderId);
            if (existingOrder == null)
            {
                return Result<UpdateRentalOrderStatusDTO>.Failure("Đơn đặt thuê không tồn tại! Kiểm tra lại Id.");
            }
            if (existingOrder.CitizenIdNavigation == null || existingOrder.DriverLicense == null)
            {
                return Result<UpdateRentalOrderStatusDTO>.Failure("Chứng minh nhân dân hoặc giấy phép lái xe chưa được nộp. Không thể cập nhật trạng thái đơn đặt thuê.");
            }
            if (existingOrder.RentalContact.Status != DocumentStatus.Approved)
            {
                return Result<UpdateRentalOrderStatusDTO>.Failure("Hợp đồng thuê xe chưa được duyệt hoặc bị từ chối. Không thể cập nhật trạng thái đơn đặt thuê.");
            }
            if (existingOrder.CitizenIdNavigation.Status != DocumentStatus.Approved)
            {
                return Result<UpdateRentalOrderStatusDTO>.Failure("Chứng minh nhân dân chưa được duyệt hoặc bị từ chối. Không thể cập nhật trạng thái đơn đặt thuê.");
            }
            if (existingOrder.DriverLicense.Status != DocumentStatus.Approved)
            {
                return Result<UpdateRentalOrderStatusDTO>.Failure("Giấy phép lái xe chưa được duyệt hoặc bị từ chối. Không thể cập nhật trạng thái đơn đặt thuê.");
            }
            existingOrder.Status = RentalOrderStatus.Confirmed;
            existingOrder.UpdatedAt = DateTime.Now;
            await _rentalOrderRepository.UpdateAsync(existingOrder);
            return Result<UpdateRentalOrderStatusDTO>.Success(updateRentalOrderStatusDTO, "Cập nhật trạng thái thành công! Mọi giấy tờ đã được duyệt!");
        }
        public async Task<Result<IEnumerable<RentalOrderDTO>>> GetByUserIdAsync(int id)
        {
            var rentalOrders = await _rentalOrderRepository.GetByUserIdAsync(id);
            var dtos = _mapper.Map<IEnumerable<RentalOrderDTO>>(rentalOrders);
            return Result<IEnumerable<RentalOrderDTO>>.Success(dtos);
        }
    }
}
