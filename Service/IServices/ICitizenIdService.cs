using Service.Common;
using Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface ICitizenIdService
    {
        Task<Result<IEnumerable<CitizenIdDTO>>> GetAllCitizenIdsAsync();
        Task<Result<CitizenIdDTO>> GetCitizenIdByIdAsync(int id);
        Task<Result<CitizenIdDTO>> GetCitizenIdByOrderIdAsync(int id);
        Task<Result<CreateCitizenIdDTO>> CreateCitizenIdAsync(CreateCitizenIdDTO createCitizenIdDTO);
        Task<Result<UpdateCitizenIdStatusDTO>> UpdateCitizenIdStatusAsync(UpdateCitizenIdStatusDTO updateCitizenIdStatusDTO);
        Task<Result<UpdateCitizenIdInfoDTO>> UpdateCitizenIdInfoAsync(UpdateCitizenIdInfoDTO updateCitizenIdInfoDTO);
    }
}
