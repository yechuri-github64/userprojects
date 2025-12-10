using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using account-mangement.Models;
using account-mangement.Services;

namespace account-mangement.Controllers
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
 _logger.LogError(ex, "Error fetching all accounts");
 return StatusCode(500, "An error occurred while fetching accounts.");
 }
 }

 [HttpGet("{id:int}")]
 public async Task<IActionResult> GetById(int id)
 {
 try
 {
 var account = await _accountService.GetByIdAsync(id);
 if (account == null) return NotFound();
 return Ok(account);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error fetching account {AccountId}", id);
 return StatusCode(500, "An error occurred while fetching the account.");
 }
 }

 // Create multiple accounts in one request
 [HttpPost]
 public async Task<IActionResult> CreateMultiple([FromBody] IEnumerable<Account> accounts)
 {
 if (accounts == null)
 {
 return BadRequest("Request body must contain a JSON array of accounts.");
 }

 try
 {
 var created = await _accountService.CreateMultipleAsync(accounts);
 return Created("api/accounts", created);
 }
 catch (ArgumentException ex)
 {
 _logger.LogWarning(ex, "Validation failed for create multiple accounts");
 return BadRequest(ex.Message);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error creating multiple accounts");
 return StatusCode(500, "An error occurred while creating accounts.");
 }
 }

 // Update exactly one account at a time
 [HttpPut("{id:int}")]
 public async Task<IActionResult> Update(int id, [FromBody] Account account)
 {
 if (account == null)
 {
 return BadRequest("Account payload is required.");
 }

 if (id != account.Id)
 {
 return BadRequest("Id in URL must match id in payload.");
 }

 try
 {
 var updated = await _accountService.UpdateAsync(account);
 if (!updated) return NotFound();
 return NoContent();
 }
 catch (ArgumentException ex)
 {
 _logger.LogWarning(ex, "Validation failed for update account {AccountId}", id);
 return BadRequest(ex.Message);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error updating account {AccountId}", id);
 return StatusCode(500, "An error occurred while updating the account.");
 }
 }
 }
}
