using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using accounts_management.Data;
using accounts_management.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace accounts_management.Services
{
    public class AccountsService : IAccountsService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<AccountsService> _logger;

        public AccountsService(ApplicationDbContext db, ILogger<AccountsService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<AccountDto>> GetAllAsync()
        {
            var list = await _db.Accounts.AsNoTracking().ToListAsync();
            return list.Select(MapToDto);
        }

        public async Task<AccountDto?> GetByIdAsync(int id)
        {
            var entity = await _db.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<AccountDto> CreateAsync(AccountCreateDto dto)
        {
            var entity = new Account
            {
                Name = dto.Name.Trim(),
                Email = dto.Email.Trim(),
                Address = dto.Address.Trim(),
            };

            _db.Accounts.Add(entity);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to create account with email {Email}", dto.Email);
                throw;
            }

            return MapToDto(entity);
        }

        public async Task<IEnumerable<AccountDto>> CreateBatchAsync(
            IEnumerable<AccountCreateDto> dtos
        )
        {
            var entities = dtos.Select(d => new Account
                {
                    Name = d.Name.Trim(),
                    Email = d.Email.Trim(),
                    Address = d.Address.Trim(),
                })
                .ToList();

            await _db.Accounts.AddRangeAsync(entities);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to create batch accounts");
                throw;
            }

            return entities.Select(MapToDto).ToList();
        }

        public async Task<AccountDto?> UpdateAsync(int id, AccountUpdateDto dto)
        {
            var entity = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == id);
            if (entity == null)
            {
                return null;
            }

            entity.Name = dto.Name.Trim();
            entity.Email = dto.Email.Trim();
            entity.Address = dto.Address.Trim();

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to update account {Id}", id);
                throw;
            }

            return MapToDto(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == id);
            if (entity == null)
            {
                return false;
            }

            _db.Accounts.Remove(entity);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to delete account {Id}", id);
                throw;
            }

            return true;
        }

        private static AccountDto MapToDto(Account a) =>
            new AccountDto
            {
                Id = a.Id,
                Name = a.Name,
                Email = a.Email,
                Address = a.Address,
            };
    }
}
