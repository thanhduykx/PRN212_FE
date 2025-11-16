using Service.Common;
using Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface IRentalOrderService
    {
        Task<Result<IEnumerable<RentalOrderDTO>>> GetAllAsync();
        Task<Result<RentalOrderDTO>> GetByIdAsync(int id);
        Task<Result<IEnumerable<RentalOrderDTO>>> GetByUserIdAsync(int id);
        Task<Result<CreateRentalOrderDTO>> CreateAsync(CreateRentalOrderDTO createRentalOrderDTO);
        Task<Result<UpdateRentalOrderStatusDTO>> UpdateStatusAsync(UpdateRentalOrderStatusDTO updateRentalOrderStatusDTO);
        Task<Result<UpdateRentalOrderTotalDTO>> UpdateTotalAsync(UpdateRentalOrderTotalDTO updateRentalOrderTotalDTO);
    }
}
