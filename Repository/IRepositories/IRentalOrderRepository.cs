using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IRentalOrderRepository
    {
        Task<IEnumerable<RentalOrder>> GetAllAsync();
        Task<RentalOrder?> GetByIdAsync(int id);
        Task<IEnumerable<RentalOrder>> GetByUserIdAsync(int id);
        Task AddAsync(RentalOrder rentalOrder);
        Task UpdateAsync(RentalOrder rentalOrder);
        Task DeleteAsync(int id);
    }
}
