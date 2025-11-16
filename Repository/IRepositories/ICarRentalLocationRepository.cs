using Microsoft.EntityFrameworkCore.Storage;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface ICarRentalLocationRepository
    {
        Task<IEnumerable<CarRentalLocation>> GetAllAsync();
        Task<CarRentalLocation?> GetByIdAsync(int id);
        Task<CarRentalLocation?> GetByCarAndLocationIdAsync(int carId, int locationId);
        Task<IEnumerable<CarRentalLocation>> GetByCarIdAsync(int id);
        Task<IEnumerable<CarRentalLocation>> GetByRentalLocationIdAsync(int id);
        Task<CarRentalLocation?> GetByCarAndRentalLocationIdAsync(int carId, int locationId);
        Task AddAsync(CarRentalLocation carRentalLocation);
        Task UpdateAsync(CarRentalLocation carRentalLocation);
        Task DeleteAsync(int id);
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
