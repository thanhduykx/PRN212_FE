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
    public class CitizenIdRepository : ICitizenIdRepository
    {
        private readonly EVSDbContext _context;
        public CitizenIdRepository(EVSDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<CitizenId>> GetAllCitizenIdsAsync()
        {
            return await Task.FromResult(_context.CitizenIds.ToList());
        }
        public async Task<CitizenId?> GetCitizenIdByIdAsync(int id)
        {
            return await _context.CitizenIds.FindAsync(id);
        }
        public async Task<CitizenId?> GetCitizenIdsByOrderIdAsync(int Id)
        {
            return await Task.FromResult(_context.CitizenIds.FirstOrDefault(c => c.RentalOrderId == Id));
        }
        public async Task AddCitizenIdAsync(CitizenId citizenId)
        {
            await _context.CitizenIds.AddAsync(citizenId);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateCitizenIdAsync(CitizenId citizenId)
        {
            _context.CitizenIds.Update(citizenId);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteCitizenIdAsync(CitizenId citizenId)
        {
            _context.CitizenIds.Remove(citizenId);
            await _context.SaveChangesAsync();
        }
    }
}
