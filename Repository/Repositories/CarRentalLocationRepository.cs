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
    public class CarRentalLocationRepository : ICarRentalLocationRepository
    {
        private readonly EVSDbContext _context;
        public CarRentalLocationRepository(EVSDbContext context)
        {
            _context = context;
        }
        public Task AddAsync(CarRentalLocation carRentalLocation)
        {
            _context.CarRentalLocations.Add(carRentalLocation);
            return _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<CarRentalLocation>> GetAllAsync()
        {
            return _context.CarRentalLocations.Where(crl => !crl.IsDeleted).ToList();
        }
        public async Task<CarRentalLocation?> GetByIdAsync(int id)
        {
            return await _context.CarRentalLocations.FindAsync(id);
        }
        public async Task<CarRentalLocation?> GetByCarAndLocationIdAsync(int carId, int locationId)
        {
            return _context.CarRentalLocations.FirstOrDefault(crl => crl.CarId == carId && crl.LocationId == locationId && !crl.IsDeleted);
        }
        public async Task<IEnumerable<CarRentalLocation>> GetByCarIdAsync(int id)
        {
            return _context.CarRentalLocations.Where(crl => crl.CarId == id && !crl.IsDeleted).ToList();
        }
        public async Task<IEnumerable<CarRentalLocation>> GetByRentalLocationIdAsync(int id)
        {
            return _context.CarRentalLocations.Where(crl => crl.LocationId == id && !crl.IsDeleted).ToList();
        }
        public Task UpdateAsync(CarRentalLocation carRentalLocation)
        {
            _context.CarRentalLocations.Update(carRentalLocation);
            return _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var carRentalLocation = await _context.CarRentalLocations.FindAsync(id);
            if (carRentalLocation != null)
            {
                carRentalLocation.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }
        public async Task<CarRentalLocation?> GetByCarAndRentalLocationIdAsync(int carId, int locationId)
        {
            return _context.CarRentalLocations.FirstOrDefault(crl => crl.CarId == carId && crl.LocationId == locationId);
        }
        public async Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
    }
}
