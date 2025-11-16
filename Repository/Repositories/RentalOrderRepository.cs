using Microsoft.EntityFrameworkCore;
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
    public class RentalOrderRepository : IRentalOrderRepository
    {
        private readonly EVSDbContext _context;
        public RentalOrderRepository(EVSDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<RentalOrder>> GetAllAsync()
        {
            return await _context.RentalOrders.ToListAsync();
        }
        public async Task<RentalOrder?> GetByIdAsync(int id)
        {
            return await _context.RentalOrders.Where(ro => ro.Id == id)
                .Include(ro => ro.CitizenIdNavigation)
                .Include(ro => ro.DriverLicense)
                .Include(ro => ro.User)
                .Include(ro => ro.Car)
                .Include(ro => ro.RentalContact)
                .Include(ro => ro.RentalLocation)
                .FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<RentalOrder>> GetByUserIdAsync(int id)
        {
            return await _context.RentalOrders.Where(ro => ro.UserId == id).ToListAsync();
        }
        public async Task AddAsync(RentalOrder rentalOrder)
        {
            await _context.RentalOrders.AddAsync(rentalOrder);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(RentalOrder rentalOrder)
        {
            _context.RentalOrders.Update(rentalOrder);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var rentalOrder = await _context.RentalOrders.FindAsync(id);
            if (rentalOrder != null)
            {
                _context.RentalOrders.Remove(rentalOrder);
                await _context.SaveChangesAsync();
            }
        }
    }
}
