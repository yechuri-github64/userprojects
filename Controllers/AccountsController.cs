using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using acc-sf-test.Services;
using acc-sf-test.Models;
using System.Collections.Generic;

namespace acc-sf-test.Controllers
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

 // Create multiple accounts in a single JSON
 [HttpPost("bulk")]
 public async Task<IActionResult> CreateBulkAsync([FromBody] IEnumerable<AccountRequest> requests)
 {
 if (requests == null)
 {
 return BadRequest(new { error = "Request body must be a JSON array of accounts." });
 }

 var result = await _accountService.CreateAccountsAsync(requests);
 return Ok(result);
 }

 // Get account by id
 [HttpGet("{id}")]
 public async Task<IActionResult> GetAsync(string id)
 {
 try
 {
 var account = await _accountService.GetAccountAsync(id);
 if (account == null) return NotFound(new { error = "Account not found." });
 return Ok(account);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error retrieving account {Id}", id);
 return StatusCode(500, new { error = ex.Message });
 }
 }

 // Update one account at a time
 [HttpPut("{id}")]
 public async Task<IActionResult> UpdateAsync(string id, [FromBody] AccountRequest request)
 {
 if (request == null) return BadRequest(new { error = "Request body is required." });
 try
 {
 var updated = await _accountService.UpdateAccountAsync(id, request);
 return Ok(updated);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error updating account {Id}", id);
 return StatusCode(500, new { error = ex.Message });
 }
 }

 [HttpDelete("{id}")]
 public async Task<IActionResult> DeleteAsync(string id)
 {
 try
 {
 await _accountService.DeleteAccountAsync(id);
 return NoContent();
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error deleting account {Id}", id);
 return StatusCode(500, new { error = ex.Message });
 }
 }
 }
}
