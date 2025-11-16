using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IDriverLicenseRepository
    {
        Task<IEnumerable<DriverLicense>> GetAllAsync();
        Task<DriverLicense?> GetByIdAsync(int id);
        Task<DriverLicense?> GetByOrderIdAsync(int id);
        Task AddAsync(DriverLicense driverLicense);
        Task UpdateAsync(DriverLicense driverLicense);
        Task DeleteAsync(int id);
    }
}
