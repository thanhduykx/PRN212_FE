using Repository.Entities;
using Service.DTOs;
using Service.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface ICarService
    {
        // 🔹 Lấy tất cả xe
        Task<Result<IEnumerable<Car>>> GetAllAsync();

        // 🔹 Lấy xe theo tên
        Task<Result<Car>> GetByNameAsync(string name);

        // 🔹 Lấy danh sách phân trang + tìm kiếm
        Task<Result<(IEnumerable<Car> Data, int Total)>> GetPagedAsync(int pageIndex, int pageSize, string? keyword = null);

        // 🔹 Thêm xe mới
        Task<Result<Car>> AddAsync(Car car);

        // 🔹 Cập nhật xe
        Task<Result<Car>> UpdateAsync(Car car);

        // 🔹 Xóa xe (mềm)
        Task<Result<bool>> DeleteAsync(int id);

        // 🔹 Lấy top xe được thuê nhiều nhất
        Task<Result<IEnumerable<TopRentCarDto>>> GetTopRentedAsync(int topCount);
    }
}
