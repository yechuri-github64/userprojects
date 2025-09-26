using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using lucky_number.Data;
using lucky_number.Models;

namespace lucky_number.Services
{
    public class GeneratedNumberService : IGeneratedNumberService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<GeneratedNumberService> _logger;

        public GeneratedNumberService(ApplicationDbContext db, ILogger<GeneratedNumberService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<GeneratedNumber> CreateAsync(GeneratedNumber item)
        {
            item.CreatedAt = DateTime.UtcNow;
            _db.GeneratedNumbers.Add(item);
            await _db.SaveChangesAsync();
            return item;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.GeneratedNumbers.FindAsync(id);
            if (entity == null) return;
            _db.GeneratedNumbers.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<GeneratedNumber>> GetAllAsync()
        {
            return await _db.GeneratedNumbers.AsNoTracking().ToListAsync();
        }

        public async Task<GeneratedNumber?> GetAsync(int id)
        {
            return await _db.GeneratedNumbers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateAsync(GeneratedNumber item)
        {
            _db.GeneratedNumbers.Update(item);
            await _db.SaveChangesAsync();
        }
    }
}
