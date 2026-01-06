using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using accounts_management_c_sharp.Models;
using accounts_management_c_sharp.Services;

namespace accounts_management_c_sharp.Controllers
{
 [ApiController]
 [Route("api/[controller]")]
 public class AccountsController : ControllerBase
 {
 private readonly IAccountService _accountService;
 private readonly ISalesforceClient _salesforceClient;
 private readonly ILogger<AccountsController> _logger;

 public AccountsController(IAccountService accountService, ISalesforceClient salesforceClient, ILogger<AccountsController> logger)
 {
 _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
 _salesforceClient = salesforceClient ?? throw new ArgumentNullException(nameof(salesforceClient));
 _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

 [HttpGet("{id}")]
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
 _logger.LogError(ex, "Error fetching account by id {Id}", id);
 return StatusCode(500, "An error occurred while fetching the account.");
 }
 }

 [HttpPost]
 public async Task<IActionResult> CreateMany([FromBody] IEnumerable<Account> accounts)
 {
 if (accounts == null)
 return BadRequest("Accounts payload is required.");

 try
 {
 var created = await _accountService.CreateManyAsync(accounts);
 return Created("/api/accounts", created);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error creating accounts");
 return StatusCode(500, "An error occurred while creating accounts.");
 }
 }

 [HttpPut("{id}")]
 public async Task<IActionResult> Update(int id, [FromBody] Account account)
 {
 if (account == null)
 return BadRequest("Account payload is required.");

 try
 {
 var updated = await _accountService.UpdateAsync(id, account);
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
 var deleted = await _accountService.DeleteAsync(id);
 if (!deleted) return NotFound();
 return NoContent();
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error deleting account {Id}", id);
 return StatusCode(500, "An error occurred while deleting the account.");
 }
 }

 // Dedicated endpoint to demonstrate Salesforce direct object operation using configured instance
 [HttpGet("salesforce/{salesforceId}")]
 public async Task<IActionResult> GetFromSalesforce(string salesforceId)
 {
 try
 {
 var resp = await _salesforceClient.GetAsync($"/services/data/v54.0/sobjects/Account/{salesforceId}");
 if (!resp.IsSuccessStatusCode) return StatusCode((int)resp.StatusCode, await resp.Content.ReadAsStringAsync());
 var content = await resp.Content.ReadAsStringAsync();
 return Content(content, "application/json");
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error fetching from Salesforce {Id}", salesforceId);
 return StatusCode(500, "An error occurred while fetching data from Salesforce.");
 }
 }
 }
}
