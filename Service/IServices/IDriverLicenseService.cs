using Repository.Entities;
using Service.Common;
using Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface IDriverLicenseService
    {
        Task<Result<IEnumerable<DriverLicenseDTO>>> GetAllAsync();
        Task<Result<DriverLicenseDTO>> GetByIdAsync(int id);
        Task<Result<DriverLicenseDTO>> GetByOrderIdAsync(int id);
        Task<Result<CreateDriverLicenseDTO>> CreateAsync(CreateDriverLicenseDTO createDriverLicenseDTO);
        Task<Result<UpdateDriverLicenseStatusDTO>> UpdateStatusAsync(UpdateDriverLicenseStatusDTO driverLicenseDTO);
        Task<Result<UpdateDriverLicenseInfoDTO>> UpdateInfoAsync(UpdateDriverLicenseInfoDTO driverLicenseDTO);
    }
}
