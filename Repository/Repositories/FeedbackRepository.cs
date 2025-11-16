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
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly EVSDbContext _context;

        public FeedbackRepository(EVSDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Feedback>> GetAllAsync()
        {
            return await _context.Feedbacks
                .Include(f => f.RentalOrder)
                .ThenInclude(o => o.Car)
                .Where(f => !f.IsDeleted)
                .ToListAsync();
        }

        // 🔍 Tìm feedback theo tên xe
        public async Task<Feedback?> GetByCarName(string carName)
        {
            return await _context.Feedbacks
                .Include(f => f.RentalOrder)
                .ThenInclude(o => o.Car)
                .Where(f => !f.IsDeleted &&
                            f.RentalOrder.Car.Name.ToLower().Contains(carName.ToLower()))
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(Feedback feedback)
        {
            await _context.Feedbacks.AddAsync(feedback);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Feedback feedback)
        {
            _context.Feedbacks.Update(feedback);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var fb = await _context.Feedbacks.FindAsync(id);
            if (fb != null)
            {
                fb.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
