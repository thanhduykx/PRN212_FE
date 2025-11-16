using Microsoft.EntityFrameworkCore.Storage;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Repository.IRepositories
{
    public interface ICarReturnHistoryRepository
    {
        Task<IEnumerable<CarReturnHistory>> GetAllAsync();
        Task<CarReturnHistory?> GetByIdAsync(int id);
        Task AddAsync(CarReturnHistory entity);
        Task UpdateAsync(CarReturnHistory entity);
        Task DeleteAsync(int id);
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}

