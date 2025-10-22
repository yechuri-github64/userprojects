using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using acc_sf_test.Services;
using acc_sf_test.Models;

namespace acc_sf_test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IAccountService accountService, ILogger<AccountsController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var accounts = await _accountService.GetAllAsync();
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all accounts");
                return StatusCode(500, new { error = "An error occurred while retrieving accounts." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var account = await _accountService.GetByIdAsync(id);
                if (account == null) return NotFound(new { error = "Account not found." });
                return Ok(account);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting account by id {Id}", id);
                return StatusCode(500, new { error = "An error occurred while retrieving the account." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] IEnumerable<AccountRequest> requests)
        {
            if (requests == null)
            {
                return BadRequest(new { error = "Request body is required and must be an array of accounts." });
            }

            try
            {
                var created = await _accountService.CreateAsync(requests);
                return Ok(created);
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, "Validation error creating accounts");
                return BadRequest(new { error = aex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating accounts");
                return StatusCode(500, new { error = "An error occurred while creating accounts." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] AccountRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { error = "Request body is required." });
            }

            try
            {
                var updated = await _accountService.UpdateAsync(id, request);
                if (updated == null) return NotFound(new { error = "Account not found." });
                return Ok(updated);
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, "Validation error updating account {Id}", id);
                return BadRequest(new { error = aex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating account {Id}", id);
                return StatusCode(500, new { error = "An error occurred while updating the account." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var deleted = await _accountService.DeleteAsync(id);
                if (!deleted) return NotFound(new { error = "Account not found." });
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting account {Id}", id);
                return StatusCode(500, new { error = "An error occurred while deleting the account." });
            }
        }
    }
}
