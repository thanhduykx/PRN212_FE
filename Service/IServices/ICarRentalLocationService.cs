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
    public interface ICarRentalLocationService
    {
        Task<Result<IEnumerable<CarRentalLocationDTO>>> GetAllAsync();
        Task<Result<IEnumerable<CarRentalLocationDTO>>> GetByCarIdAsync(int id);
        Task<Result<IEnumerable<CarRentalLocationDTO>>> GetByRentalLocationIdAsync(int id);
        Task<Result<CarRentalLocationDTO>> GetByIdAsync(int id);
        Task<Result<CreateCarRentalLocationDTO>> AddAsync(CreateCarRentalLocationDTO carRentalLocationDTO);
        Task<Result<UpdateCarRentalLocationDTO>> UpdateAsync(UpdateCarRentalLocationDTO carRentalLocationDTO);
        Task DeleteAsync(int id);
    }
}
