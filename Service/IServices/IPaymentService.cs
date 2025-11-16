using Service.Common;
using Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface IPaymentService
    {
        Task<Result<IEnumerable<PaymentDTO>>> GetAllAsync();
        Task<Result<IEnumerable<PaymentDTO>>> GetAllByUserIdAsync(int id);
        Task<Result<PaymentDTO>> GetByIdAsync(int id);
        Task<Result<CreatePaymentDTO>> AddAsync(CreatePaymentDTO createPaymentDTO);
        Task<Result<UpdatePaymentStatusDTO>> UpdatePaymentStatusAsync(UpdatePaymentStatusDTO updatePaymentDTO);
    }
}
