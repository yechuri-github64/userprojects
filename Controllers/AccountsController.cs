using accounts_management.Models;
using accounts_management.Services;
using Microsoft.AspNetCore.Mvc;

namespace accounts_management.Controllers
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
        public async Task<ActionResult<IEnumerable<Account>>> GetAll()
        {
            try
            {
                var accounts = await _service.GetAllAsync();
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching accounts");
                return StatusCode(500, "An error occurred while fetching accounts.");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Account>> GetById(int id)
        {
            try
            {
                var account = await _service.GetByIdAsync(id);
                if (account == null)
                    return NotFound();
                return Ok(account);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching account with id {Id}", id);
                return StatusCode(500, "An error occurred while fetching the account.");
            }
        }

        [HttpPost("bulk")]
        public async Task<ActionResult<IEnumerable<Account>>> CreateBulk(
            [FromBody] IEnumerable<Account> accounts
        )
        {
            if (accounts == null)
                return BadRequest("Request body cannot be null.");

            try
            {
                var created = await _service.CreateBulkAsync(accounts);
                return Ok(created);
            }
            catch (ArgumentException argEx)
            {
                _logger.LogWarning(argEx, "Validation error during bulk create");
                return BadRequest(argEx.Message);
            }
            catch (InvalidOperationException invEx)
            {
                _logger.LogWarning(invEx, "Conflict during bulk create");
                return Conflict(invEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating accounts in bulk");
                return StatusCode(500, "An error occurred while creating accounts.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Account>> Create([FromBody] Account account)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var created = await _service.CreateAsync(account);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ArgumentException argEx)
            {
                _logger.LogWarning(argEx, "Validation error during create");
                return BadRequest(argEx.Message);
            }
            catch (InvalidOperationException invEx)
            {
                _logger.LogWarning(invEx, "Conflict during create");
                return Conflict(invEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating account");
                return StatusCode(500, "An error occurred while creating the account.");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Account>> Update(int id, [FromBody] Account account)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var updated = await _service.UpdateAsync(id, account);
                if (updated == null)
                    return NotFound();
                return Ok(updated);
            }
            catch (ArgumentException argEx)
            {
                _logger.LogWarning(argEx, "Validation error during update for id {Id}", id);
                return BadRequest(argEx.Message);
            }
            catch (InvalidOperationException invEx)
            {
                _logger.LogWarning(invEx, "Conflict during update for id {Id}", id);
                return Conflict(invEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating account with id {Id}", id);
                return StatusCode(500, "An error occurred while updating the account.");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);
                if (!deleted)
                    return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting account with id {Id}", id);
                return StatusCode(500, "An error occurred while deleting the account.");
            }
        }
    }
}
