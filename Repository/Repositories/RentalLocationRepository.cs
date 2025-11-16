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
    public class RentalLocationRepository : IRentalLocationRepository
    {
        private readonly EVSDbContext _context;
        public RentalLocationRepository(EVSDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<RentalLocation>> GetAllAsync()
        {
            return await _context.RentalLocations.Where(rl => !rl.IsDeleted).ToListAsync();
        }
        public async Task<IEnumerable<User>> GetAllStaffByLocationAsync(int id)
        {
            return await _context.Users
                .Where(u => u.RentalLocationId == id && u.IsActive)
                .ToListAsync();
        }
        public async Task<RentalLocation?> GetByIdAsync(int id)
        {
            return await _context.RentalLocations.FirstOrDefaultAsync(rl => rl.Id == id && !rl.IsDeleted);
        }
        public async Task AddAsync(RentalLocation rentalLocation)
        {
            _context.RentalLocations.AddAsync(rentalLocation);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(RentalLocation rentalLocation)
        {
            _context.RentalLocations.Update(rentalLocation);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var rentalLocation = await GetByIdAsync(id);
            if (rentalLocation != null)
            {
                rentalLocation.IsDeleted = true;
                await UpdateAsync(rentalLocation);
            }
        }
        public async Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
    }
}
