using Service.Common;
using Service.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface ICarDeliveryHistoryService
    {
        Task<Result<(IEnumerable<CarDeliveryHistoryDTO> Data, int Total)>> GetAllAsync(int pageIndex, int pageSize);
        Task<Result<CarDeliveryHistoryDTO?>> GetByIdAsync(int id);
        Task<Result<string>> AddAsync(CarDeliveryHistoryCreateDTO dto);
        Task<Result<string>> UpdateAsync(int id, CarDeliveryHistoryCreateDTO dto);
        Task<Result<string>> DeleteAsync(int id);
    }

}
