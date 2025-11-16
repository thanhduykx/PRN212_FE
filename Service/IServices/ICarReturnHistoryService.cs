using Service.Common;
using Service.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface ICarReturnHistoryService
    {
        Task<Result<IEnumerable<CarReturnHistoryDTO>>> GetAllAsync();
        Task<Result<CarReturnHistoryDTO?>> GetByIdAsync(int id);
        Task<Result<string>> AddAsync(CarReturnHistoryCreateDTO dto);
        Task<Result<string>> UpdateAsync(int id, CarReturnHistoryCreateDTO dto);
        Task<Result<string>> DeleteAsync(int id);
    }
}
