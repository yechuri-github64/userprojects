using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using test-acc-sf-app.Models;
using test-acc-sf-app.Services;

namespace test-acc-sf-app.Controllers
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

 [HttpPost]
 public async Task<IActionResult> CreateAccounts([FromBody] List<AccountCreateRequest> requests)
 {
 if (requests == null || requests.Count == 0)
 {
 return BadRequest(new { error = "Request body must be a non-empty JSON array of accounts." });
 }

 try
 {
 var result = await _accountService.CreateAccountsAsync(requests);
 return StatusCode((int)HttpStatusCode.Created, result);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error creating accounts");
 return StatusCode(500, new { error = "An error occurred while creating accounts." });
 }
 }

 [HttpGet("{id}")]
 public async Task<IActionResult> GetAccount(string id)
 {
 if (string.IsNullOrWhiteSpace(id))
 {
 return BadRequest(new { error = "Account id is required." });
 }

 try
 {
 var account = await _accountService.GetAccountAsync(id);
 if (account == null)
 {
 return NotFound();
 }

 return Ok(account);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error retrieving account {AccountId}", id);
 return StatusCode(500, new { error = "An error occurred while retrieving the account." });
 }
 }

 [HttpPut("{id}")]
 public async Task<IActionResult> UpdateAccount(string id, [FromBody] AccountUpdateRequest update)
 {
 if (string.IsNullOrWhiteSpace(id) || update == null)
 {
 return BadRequest(new { error = "Account id and update data are required." });
 }

 try
 {
 var updated = await _accountService.UpdateAccountAsync(id, update);
 if (!updated)
 {
 return NotFound();
 }

 return NoContent();
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error updating account {AccountId}", id);
 return StatusCode(500, new { error = "An error occurred while updating the account." });
 }
 }

 [HttpDelete("{id}")]
 public async Task<IActionResult> DeleteAccount(string id)
 {
 if (string.IsNullOrWhiteSpace(id))
 {
 return BadRequest(new { error = "Account id is required." });
 }

 try
 {
 var deleted = await _accountService.DeleteAccountAsync(id);
 if (!deleted)
 {
 return NotFound();
 }

 return NoContent();
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error deleting account {AccountId}", id);
 return StatusCode(500, new { error = "An error occurred while deleting the account." });
 }
 }
 }
}
