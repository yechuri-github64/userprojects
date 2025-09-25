using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using test_project_main1.Data;
using test_project_main1.Models;

namespace test_project_main1.Services
{
    public class NumberService : INumberService
    {
        private readonly ApplicationDbContext _db;

        public NumberService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Number> CreateAsync(Number number)
        {
            if (number == null) throw new ArgumentException("Number cannot be null.");

            _db.Numbers.Add(number);
            await _db.SaveChangesAsync();
            return number;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _db.Numbers.FindAsync(id);
            if (existing == null) return false;
            _db.Numbers.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Number>> GetAllAsync()
        {
            return await _db.Numbers.AsNoTracking().ToListAsync();
        }

        public async Task<Number?> GetByIdAsync(int id)
        {
            return await _db.Numbers.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<IEnumerable<Number>> GetDivisibleBy5AndLessThan500Async()
        {
            return await _db.Numbers
                .AsNoTracking()
                .Where(n => n.Value % 5 == 0 && n.Value < 500)
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(Number number)
        {
            var existing = await _db.Numbers.FindAsync(number.Id);
            if (existing == null) return false;
            existing.Value = number.Value;
            _db.Numbers.Update(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
