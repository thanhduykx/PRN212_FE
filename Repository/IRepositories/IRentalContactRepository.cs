using Repository.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IRentalContactRepository
    {
        Task<IEnumerable<RentalContact>> GetAllAsync();
        Task<RentalContact?> GetByIdAsync(int id);
        Task<RentalContact?> GetByRentalOrderIdAsync(int rentalOrderId); // 🔍 get by RentalOrderId
        Task AddAsync(RentalContact contact);
        Task UpdateAsync(RentalContact contact);
        Task DeleteAsync(int id);
    }
}
