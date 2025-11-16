using Repository.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface ICarRepository
    {
        Task<IEnumerable<Car>> GetAllAsync();
        Task<Car?> GetByIdAsync(int id);
        Task<Car> GetByNameAsync(string name);
        Task AddAsync(Car car);
        Task UpdateAsync(Car car);
        Task DeleteAsync(int id);

        // Lấy top thuê nhiều nhất, chỉ entity
        Task<IEnumerable<Car>> GetTopRentedAsync(int topCount);
    }
}
