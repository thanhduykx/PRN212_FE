using Service.Common;
using Service.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface IRentalContactService
    {
        Task<Result<IEnumerable<RentalContactDTO>>> GetAllAsync();
        Task<Result<RentalContactDTO?>> GetByRentalOrderIdAsync(int rentalOrderId);
        Task<Result<string>> AddAsync(RentalContactCreateDTO dto);
        Task<Result<string>> UpdateAsync(RentalContactUpdateDTO dto);
        Task<Result<string>> DeleteAsync(int id);
    }
}
