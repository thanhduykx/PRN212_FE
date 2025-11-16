using Repository.Entities;
using Repository.IRepositories;
using Service.Common;
using Service.IServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;

        public FeedbackService(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }

        // 🔹 Lấy tất cả feedback
        public async Task<Result<IEnumerable<Feedback>>> GetAllAsync()
        {
            var list = await _feedbackRepository.GetAllAsync();
            return Result<IEnumerable<Feedback>>.Success(list);
        }

        // 🔹 Lấy feedback theo tên xe
        public async Task<Result<Feedback>> GetByCarNameAsync(string carName)
        {
            var feedback = await _feedbackRepository.GetByCarName(carName);
            if (feedback == null)
                return Result<Feedback>.Failure("Không tìm thấy feedback cho xe này.");

            return Result<Feedback>.Success(feedback);
        }

        // 🔹 Lấy danh sách feedback có phân trang và tìm kiếm
        public async Task<Result<(IEnumerable<Feedback> Data, int Total)>> GetPagedAsync(int pageIndex, int pageSize, string? keyword = null)
        {
            var allFeedbacks = await _feedbackRepository.GetAllAsync();
            var filtered = allFeedbacks.Where(f => !f.IsDeleted);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                filtered = filtered.Where(f =>
                    (f.Content != null && f.Content.Contains(keyword)) ||
                    (f.Title != null && f.Title.Contains(keyword))
                );
            }

            var total = filtered.Count();

            var data = filtered
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Result<(IEnumerable<Feedback> Data, int Total)>.Success((data, total));
        }

        // 🔹 Thêm feedback
        public async Task<Result<bool>> AddAsync(Feedback feedback)
        {
            await _feedbackRepository.AddAsync(feedback);
            return Result<bool>.Success(true, "Thêm feedback thành công.");
        }

        // 🔹 Cập nhật feedback
        public async Task<Result<bool>> UpdateAsync(Feedback feedback)
        {
            await _feedbackRepository.UpdateAsync(feedback);
            return Result<bool>.Success(true, "Cập nhật feedback thành công.");
        }

        // 🔹 Xóa mềm feedback
        public async Task<Result<bool>> DeleteAsync(int id)
        {
            await _feedbackRepository.DeleteAsync(id);
            return Result<bool>.Success(true, "Xóa feedback thành công.");
        }
    }
}
