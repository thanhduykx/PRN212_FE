
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface IAIService
    {
        /// <summary>
        /// Gọi AI để tạo phản hồi văn bản dựa trên prompt
        /// </summary>
        Task<string> GenerateResponseAsync(string prompt, string model = "flash", bool shortAnswer = false);

        /// <summary>
        /// Phân tích tỷ lệ sử dụng xe và giờ cao điểm dựa trên dữ liệu trong DB
        /// </summary>
        Task<string> AnalyzeCarUsageAsync(string model = "flash", bool shortAnswer = false);
    }
}
