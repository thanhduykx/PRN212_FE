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
    public class DriverLicenseRepository : IDriverLicenseRepository
    {
        private readonly EVSDbContext _context;
        public DriverLicenseRepository(EVSDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<DriverLicense>> GetAllAsync()
        {
            return await Task.FromResult(_context.DriverLicenses.ToList());
        }
        public async Task<DriverLicense?> GetByIdAsync(int id)
        {
            return await _context.DriverLicenses.FindAsync(id);
        }
        public async Task<DriverLicense?> GetByOrderIdAsync(int id)
        {
            return await Task.FromResult(_context.DriverLicenses.FirstOrDefault(dl => dl.RentalOrderId == id));
        }
        public async Task AddAsync(DriverLicense driverLicense)
        {
            await _context.DriverLicenses.AddAsync(driverLicense);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(DriverLicense driverLicense)
        {
            _context.DriverLicenses.Update(driverLicense);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var driverLicense = await _context.DriverLicenses.FindAsync(id);
            if (driverLicense != null)
            {
                _context.DriverLicenses.Remove(driverLicense);
                await _context.SaveChangesAsync();
            }
        }
    }
}
