using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Amazon.Lambda.Core;
using DatatableLambda.Services;
using DatatableLambda.Data;
using DatatableLambda.Models;

namespace DatatableLambda
{
    public class Function
    {
        private readonly IServiceProvider _provider;

        public Function()
        {
            var services = new ServiceCollection();
            Service.ConfigureServices(services);
            _provider = services.BuildServiceProvider();
        }

        /// <summary>
        /// Lambda handler named "datatable" as required.
        /// Accepts a Request and returns a Response containing accounts or structured error.
        /// </summary>
        public async Task<Response> datatable(Request request, ILambdaContext context)
        {
            try
            {
                using var scope = _provider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Apply pagination if provided
                var query = db.Accounts.AsNoTracking().AsQueryable();

                if (request?.Offset.HasValue == true)
                {
                    query = query.Skip(request.Offset.Value);
                }
                if (request?.Limit.HasValue == true)
                {
                    query = query.Take(request.Limit.Value);
                }

                var accounts = await query.ToListAsync();

                var data = accounts.Select(a => new Response.AccountDto
                {
                    Id = a.Id,
                    Username = a.Username,
                    Email = a.Email,
                    CreatedAt = a.CreatedAt
                }).ToList();

                return new Response
                {
                    Success = true,
                    Data = data,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                // Log the error
                try
                {
                    context?.Logger?.LogLine($"Error in datatable handler: {ex.Message}\n{ex.StackTrace}");
                }
                catch { /* swallow logging errors */ }

                var err = new Response.ErrorInfo
                {
                    Code = "InternalError",
                    Message = ex.Message,
                    Details = ex.StackTrace,
                    Timestamp = DateTime.UtcNow
                };

                return new Response
                {
                    Success = false,
                    Data = new List<Response.AccountDto>(),
                    Error = err
                };
            }
        }
    }
}
