using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Repository.Context;
using Repository.Entities;
using Repository.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class CarReturnHistoryRepository : ICarReturnHistoryRepository
    {
        private readonly EVSDbContext _context;

        public CarReturnHistoryRepository(EVSDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CarReturnHistory>> GetAllAsync()
        {
            return await _context.CarReturnHistories
                .Include(x => x.Order)
                .Include(x => x.Customer)
                .Include(x => x.Staff)
                .Include(x => x.Car)
                .ToListAsync();
        }

        public async Task<CarReturnHistory?> GetByIdAsync(int id)
        {
            return await _context.CarReturnHistories
                .Include(x => x.Order)
                .Include(x => x.Customer)
                .Include(x => x.Staff)
                .Include(x => x.Car)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(CarReturnHistory entity)
        {
            await _context.CarReturnHistories.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CarReturnHistory entity)
        {
            _context.CarReturnHistories.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.CarReturnHistories.FindAsync(id);
            if (entity != null)
            {
                _context.CarReturnHistories.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
    }
}
