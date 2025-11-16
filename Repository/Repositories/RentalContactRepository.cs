using Microsoft.EntityFrameworkCore;
using Repository.Context;
using Repository.Entities;
using Repository.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class RentalContactRepository : IRentalContactRepository
    {
        private readonly EVSDbContext _context;

        public RentalContactRepository(EVSDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RentalContact>> GetAllAsync()
        {
            return await _context.RentalContacts
                .Include(c => c.RentalOrder)
                .Where(c => !c.IsDeleted)
                .ToListAsync();
        }
        public async Task<RentalContact?> GetByIdAsync(int id)
        {
            return await _context.RentalContacts
                .Include(c => c.RentalOrder)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }
        public async Task<RentalContact?> GetByRentalOrderIdAsync(int rentalOrderId)
        {
            return await _context.RentalContacts
                .Include(c => c.RentalOrder)
                .FirstOrDefaultAsync(c => c.RentalOrderId == rentalOrderId && !c.IsDeleted);
        }

        public async Task AddAsync(RentalContact contact)
        {
            await _context.RentalContacts.AddAsync(contact);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RentalContact contact)
        {
            _context.RentalContacts.Update(contact);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var contact = await _context.RentalContacts.FindAsync(id);
            if (contact != null)
            {
                contact.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
