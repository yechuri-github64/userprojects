using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatatableLambda.Data;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

namespace DatatableLambda.Services
{
    public class Service
    {
        // Followed the required fields and initialization pattern
        private string _conn;
        private ApplicationDbContext? _db;

        public Service()
        {
            // Load env file
            Env.Load();
            string? conn = Env.GetString("MYSQL_CONN") ?? Environment.GetEnvironmentVariable("MYSQL_CONN");
            if (conn == null) throw new Exception("No conn");
            _conn = conn!;

            // initialize to satisfy nullable warnings
            _db = null!;

            // Create DbContext with the connection
            _db = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseMySql(_conn, ServerVersion.AutoDetect(_conn)).Options);
        }

        public async Task<List<Account>> GetAllAccountsAsync()
        {
            if (_db == null) throw new Exception("Database context not initialized");

            try
            {
                return await _db.Accounts.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
