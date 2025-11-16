using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Repository.IRepositories
{
    public interface ICarDeliveryHistoryRepository
    {
        Task<IEnumerable<CarDeliveryHistory>> GetAllAsync(int pageIndex, int pageSize);
        Task<int> CountAsync();
        Task<CarDeliveryHistory?> GetByIdAsync(int id);
        Task AddAsync(CarDeliveryHistory entity);
        Task UpdateAsync(CarDeliveryHistory entity);
        Task DeleteAsync(int id);
    }
}

