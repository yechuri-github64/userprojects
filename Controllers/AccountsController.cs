using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using account_c_sharp.Services;
using account_c_sharp.Models;

namespace account_c_sharp.Controllers
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
 _logger.LogError(ex, "Error while getting all accounts");
 return StatusCode(500, "An error occurred while processing your request.");
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
 _logger.LogError(ex, "Error while getting account by id: {Id}", id);
 return StatusCode(500, "An error occurred while processing your request.");
 }
 }

 [HttpPost]
 public async Task<IActionResult> CreateMany([FromBody] IEnumerable<Account> accounts)
 {
 if (accounts == null)
 return BadRequest("Request body must be a JSON array of accounts.");

 try
 {
 var created = await _accountService.CreateManyAsync(accounts);
 return Ok(created);
 }
 catch (ArgumentException aex)
 {
 _logger.LogWarning(aex, "Validation failed for bulk create");
 return BadRequest(aex.Message);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error while creating accounts");
 return StatusCode(500, "An error occurred while processing your request.");
 }
 }

 [HttpPut("{id:int}")]
 public async Task<IActionResult> Update(int id, [FromBody] Account account)
 {
 if (account == null) return BadRequest("Account object is required.");
 if (id != account.Id) return BadRequest("Route id and account id must match.");

 try
 {
 var updated = await _accountService.UpdateAsync(account);
 if (!updated) return NotFound();
 return NoContent();
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error while updating account id: {Id}", id);
 return StatusCode(500, "An error occurred while processing your request.");
 }
 }

 [HttpDelete("{id:int}")]
 public async Task<IActionResult> Delete(int id)
 {
 try
 {
 var deleted = await _accountService.DeleteAsync(id);
 if (!deleted) return NotFound();
 return NoContent();
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error while deleting account id: {Id}", id);
 return StatusCode(500, "An error occurred while processing your request.");
 }
 }
 }
}
