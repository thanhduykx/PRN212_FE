using Service.Common;
using Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface IRentalLocationService
    {
        Task<Result<IEnumerable<RentalLocationDTO>>> GetAllAsync();
        Task<Result<IEnumerable<UserDTO>>> GetAllStaffByLocationIdAsync(int id);
        Task<Result<RentalLocationDTO>> GetByIdAsync(int id);
        Task<Result<CreateRentalLocationDTO>> AddAsync(CreateRentalLocationDTO rentalLocationDTO);
        Task<Result<UpdateRentalLocationDTO>> UpdateAsync(UpdateRentalLocationDTO rentalLocationDTO);
        Task DeleteAsync(int id);
    }
}
