using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using accounts_operation.Services;
using accounts_operation.Models;

namespace accounts_operation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _service;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IAccountService service, ILogger<AccountsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> Get()
        {
            try
            {
                var items = await _service.GetAllAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting accounts");
                return StatusCode(500, "An error occurred while retrieving accounts.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> Get(int id)
        {
            try
            {
                var item = await _service.GetByIdAsync(id);
                if (item == null) return NotFound();
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting account {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the account.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Account>> Create([FromBody] Account account)
        {
            try
            {
                var created = await _service.CreateAsync(account);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating account");
                return StatusCode(500, "An error occurred while creating the account.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Account account)
        {
            try
            {
                if (id != account.Id) return BadRequest("Id mismatch.");
                var updated = await _service.UpdateAsync(account);
                if (!updated) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating account {Id}", id);
                return StatusCode(500, "An error occurred while updating the account.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);
                if (!deleted) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting account {Id}", id);
                return StatusCode(500, "An error occurred while deleting the account.");
            }
        }
    }
}
