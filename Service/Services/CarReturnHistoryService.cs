using AutoMapper;
using Repository.Entities;
using Repository.Entities.Enum;
using Repository.IRepositories;
using Service.Common;
using Service.DTOs;
using Service.IServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Service.Services
{
    public class CarReturnHistoryService : ICarReturnHistoryService
    {
        private readonly ICarReturnHistoryRepository _repo;
        private readonly ICarRentalLocationRepository _carRentalLocationRepo;
        private readonly IRentalOrderRepository _rentalOrderRepo;
        private readonly IMapper _mapper;

        public CarReturnHistoryService(
            ICarReturnHistoryRepository repo,
            ICarRentalLocationRepository carRentalLocationRepo,
            IRentalOrderRepository rentalOrderRepo,
            IMapper mapper)
        {
            _repo = repo;
            _carRentalLocationRepo = carRentalLocationRepo;
            _rentalOrderRepo = rentalOrderRepo;
            _mapper = mapper;
        }

        // 🔹 Lấy tất cả
        public async Task<Result<IEnumerable<CarReturnHistoryDTO>>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            var mapped = _mapper.Map<IEnumerable<CarReturnHistoryDTO>>(list);
            return Result<IEnumerable<CarReturnHistoryDTO>>.Success(mapped, "Lấy danh sách lịch sử trả xe thành công.");
        }

        // 🔹 Lấy 1 bản ghi theo Id
        public async Task<Result<CarReturnHistoryDTO?>> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                return Result<CarReturnHistoryDTO?>.Failure("Không tìm thấy lịch sử trả xe.");

            return Result<CarReturnHistoryDTO?>.Success(_mapper.Map<CarReturnHistoryDTO>(entity));
        }

        // 🔹 Thêm mới (Xử lý logic trả xe)
        public async Task<Result<string>> AddAsync(CarReturnHistoryCreateDTO dto)
        {
            using var transaction = await _carRentalLocationRepo.BeginTransactionAsync();

            try
            {
                // Lấy order để xác định xe & chi nhánh
                var order = await _rentalOrderRepo.GetByIdAsync(dto.OrderId);
                if (order == null)
                    return Result<string>.Failure("Không tìm thấy đơn hàng để trả xe.");

                // Lấy thông tin xe tại chi nhánh
                var carLocation = await _carRentalLocationRepo.GetByCarAndRentalLocationIdAsync(order.CarId, order.RentalLocationId);
                if (carLocation == null)
                    return Result<string>.Failure("Không tìm thấy xe tại chi nhánh để cập nhật.");

                // +1 xe về chi nhánh
                carLocation.Quantity += 1;
                await _carRentalLocationRepo.UpdateAsync(carLocation);

                // Cập nhật trạng thái đơn hàng

                order.ActualReturnTime = dto.ReturnDate;
                await _rentalOrderRepo.UpdateAsync(order);

                // Lưu lịch sử trả xe
                var entity = new CarReturnHistory
                {
                    ReturnDate = dto.ReturnDate,
                    OdometerEnd = dto.OdometerEnd,
                    BatteryLevelEnd = dto.BatteryLevelEnd,
                    VehicleConditionEnd = dto.VehicleConditionEnd,
                    OrderId = dto.OrderId
                };
                await _repo.AddAsync(entity);

                await transaction.CommitAsync();
                return Result<string>.Success("OK", "✅ Trả xe thành công, đơn hàng đã cập nhật sang trạng thái 'Returned'.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Result<string>.Failure($"❌ Trả xe thất bại: {ex.Message}");
            }
        }

        // 🔹 Cập nhật
        public async Task<Result<string>> UpdateAsync(int id, CarReturnHistoryCreateDTO dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return Result<string>.Failure("Không tìm thấy lịch sử trả xe.");

            _mapper.Map(dto, existing);
            await _repo.UpdateAsync(existing);
            return Result<string>.Success("OK", "Cập nhật lịch sử trả xe thành công.");
        }

        // 🔹 Xóa
        public async Task<Result<string>> DeleteAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return Result<string>.Failure("Không tìm thấy lịch sử trả xe để xóa.");

            await _repo.DeleteAsync(id);
            return Result<string>.Success("OK", "Xóa lịch sử trả xe thành công.");
        }
    }
}
