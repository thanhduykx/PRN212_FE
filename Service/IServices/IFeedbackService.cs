using Repository.Entities;
using Service.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface IFeedbackService
    {
        // 🔹 Lấy tất cả feedback
        Task<Result<IEnumerable<Feedback>>> GetAllAsync();

        // 🔹 Tìm feedback theo tên xe
        Task<Result<Feedback>> GetByCarNameAsync(string carName);

        // 🔹 Lấy danh sách feedback có phân trang
        Task<Result<(IEnumerable<Feedback> Data, int Total)>> GetPagedAsync(int pageIndex, int pageSize, string? keyword = null);

        // 🔹 Thêm feedback
        Task<Result<bool>> AddAsync(Feedback feedback);

        // 🔹 Cập nhật feedback
        Task<Result<bool>> UpdateAsync(Feedback feedback);

        // 🔹 Xóa  feedback
        Task<Result<bool>> DeleteAsync(int id);
    }
}
