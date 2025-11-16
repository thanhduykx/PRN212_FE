using Microsoft.EntityFrameworkCore;
using Repository.Context;
using Repository.Entities;
using Repository.IRepositories;


namespace Repository.Repositories
{
    public class CarDeliveryHistoryRepository : ICarDeliveryHistoryRepository
    {
        private readonly EVSDbContext _context;

        public CarDeliveryHistoryRepository(EVSDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CarDeliveryHistory>> GetAllAsync(int pageIndex, int pageSize)
        {
            return await _context.CarDeliveryHistories
                .Include(x => x.Car)
                .Include(x => x.Customer)
                .Include(x => x.Staff)
                .Include(x => x.Location)
                .OrderByDescending(x => x.DeliveryDate)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.CarDeliveryHistories.CountAsync();
        }

        public async Task<CarDeliveryHistory?> GetByIdAsync(int id)
        {
            return await _context.CarDeliveryHistories
                .Include(x => x.Car)
                .Include(x => x.Customer)
                .Include(x => x.Staff)
                .Include(x => x.Location)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(CarDeliveryHistory entity)
        {
            await _context.CarDeliveryHistories.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CarDeliveryHistory entity)
        {
            _context.CarDeliveryHistories.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.CarDeliveryHistories.FindAsync(id);
            if (entity != null)
            {
                _context.CarDeliveryHistories.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
    }
