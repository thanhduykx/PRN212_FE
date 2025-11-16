using AutoMapper;
using Repository.Entities;
using Repository.Entities.Enum;
using Repository.IRepositories;
using Service.DTOs;
using Service.IServices;
using Service.Common;

namespace Service.Services
{
    public class CarDeliveryHistoryService : ICarDeliveryHistoryService
    {
        private readonly ICarDeliveryHistoryRepository _repo;
        private readonly ICarRentalLocationRepository _carRentalLocationRepo;
        private readonly IRentalOrderRepository _rentalOrderRepo;
        private readonly IMapper _mapper;

        public CarDeliveryHistoryService(
            ICarDeliveryHistoryRepository repo,
            ICarRentalLocationRepository carRentalLocationRepo,
            IRentalOrderRepository rentalOrderRepo,
            IMapper mapper)
        {
            _repo = repo;
            _carRentalLocationRepo = carRentalLocationRepo;
            _rentalOrderRepo = rentalOrderRepo;
            _mapper = mapper;
        }

        // 🔹 Lấy tất cả lịch sử giao xe (có phân trang)
        public async Task<Result<(IEnumerable<CarDeliveryHistoryDTO> Data, int Total)>> GetAllAsync(int pageIndex, int pageSize)
        {
            try
            {
                var list = await _repo.GetAllAsync(pageIndex, pageSize);
                var total = await _repo.CountAsync();

                var mapped = _mapper.Map<IEnumerable<CarDeliveryHistoryDTO>>(list);
                return Result<(IEnumerable<CarDeliveryHistoryDTO>, int)>.Success((mapped, total), "Lấy danh sách lịch sử giao xe thành công.");
            }
            catch (Exception ex)
            {
                return Result<(IEnumerable<CarDeliveryHistoryDTO>, int)>.Failure($"Lỗi khi lấy danh sách: {ex.Message}");
            }
        }

        // 🔹 Lấy 1 bản ghi theo ID
        public async Task<Result<CarDeliveryHistoryDTO?>> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity == null)
                    return Result<CarDeliveryHistoryDTO?>.Failure("Không tìm thấy lịch sử giao xe.");

                return Result<CarDeliveryHistoryDTO?>.Success(_mapper.Map<CarDeliveryHistoryDTO>(entity), "Lấy thông tin thành công.");
            }
            catch (Exception ex)
            {
                return Result<CarDeliveryHistoryDTO?>.Failure($"Lỗi khi lấy dữ liệu: {ex.Message}");
            }
        }

        // 🔹 Thêm lịch sử giao xe
        public async Task<Result<string>> AddAsync(CarDeliveryHistoryCreateDTO dto)
        {
            try
            {
                // 🔍 Lấy thông tin đơn thuê
                var order = await _rentalOrderRepo.GetByIdAsync(dto.OrderId);
                if (order == null)
                    return Result<string>.Failure("Không tìm thấy đơn thuê.");

                // 🔍 Kiểm tra xe có sẵn ở chi nhánh không
                var carRentalLocation = await _carRentalLocationRepo
                    .GetByCarAndRentalLocationIdAsync(order.CarId, order.RentalLocationId);

                if (carRentalLocation == null)
                    return Result<string>.Failure("Không tìm thấy xe tại chi nhánh này.");

                if (carRentalLocation.Quantity <= 0)
                    return Result<string>.Failure("Chi nhánh này không còn xe khả dụng để giao.");

                using var transaction = await _carRentalLocationRepo.BeginTransactionAsync();

                // 🚗 Giảm số lượng xe khả dụng tại chi nhánh
                carRentalLocation.Quantity -= 1;
                await _carRentalLocationRepo.UpdateAsync(carRentalLocation);

                // 📝 Tạo bản ghi giao xe
                var history = new CarDeliveryHistory
                {
                    DeliveryDate = dto.DeliveryDate,
                    OdometerStart = dto.OdometerStart,
                    BatteryLevelStart = dto.BatteryLevelStart,
                    VehicleConditionStart = dto.VehicleConditionStart,
                    OrderId = order.Id,
                    CustomerId = order.UserId,
                    StaffId = 0, // Có thể cập nhật theo user đăng nhập
                    CarId = order.CarId,
                    LocationId = order.RentalLocationId
                };

                await _repo.AddAsync(history);

                // 🔁 Cập nhật trạng thái đơn hàng sang "Renting"
                order.Status = RentalOrderStatus.Renting;
                await _rentalOrderRepo.UpdateAsync(order);

                await transaction.CommitAsync();
                return Result<string>.Success("✅ Giao xe thành công. Trạng thái đơn đã chuyển sang 'Renting'.");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"❌ Giao xe thất bại: {ex.Message}");
            }
        }

        // 🔹 Cập nhật lịch sử giao xe
        public async Task<Result<string>> UpdateAsync(int id, CarDeliveryHistoryCreateDTO dto)
        {
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity == null)
                    return Result<string>.Failure("Không tìm thấy lịch sử giao xe cần cập nhật.");

                _mapper.Map(dto, entity);
                await _repo.UpdateAsync(entity);

                return Result<string>.Success("Cập nhật lịch sử giao xe thành công.");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Lỗi khi cập nhật: {ex.Message}");
            }
        }

        // 🔹 Xóa lịch sử giao xe
        public async Task<Result<string>> DeleteAsync(int id)
        {
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity == null)
                    return Result<string>.Failure("Không tìm thấy lịch sử giao xe cần xóa.");

                await _repo.DeleteAsync(id);
                return Result<string>.Success("Xóa lịch sử giao xe thành công.");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Lỗi khi xóa: {ex.Message}");
            }
        }
    }
}
