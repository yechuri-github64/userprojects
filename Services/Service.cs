using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DatatableLambda.Data;
using DotNetEnv;

namespace DatatableLambda.Services
{
    public class Service
    {
        private string _conn;
        private ApplicationDbContext? _db;

        public Service()
        {
            // satisfy nullable analyzer initially
            _conn = null!;
            _db = null!;

            // Load env vars
            Env.Load();
            string? conn = Env.GetString("MYSQL_CONN") ?? Environment.GetEnvironmentVariable("MYSQL_CONN");
            if (conn == null)
                throw new Exception("No conn");

            _conn = conn!;

            // Initialize DbContext
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseMySql(_conn, ServerVersion.AutoDetect(_conn))
                .Options;

            _db = new ApplicationDbContext(options, _conn);
        }

        public async Task<List<Account>> GetAccountsAsync()
        {
            try
            {
                if (_db == null)
                    throw new Exception("Database context not initialized");

                return await _db.Accounts.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
