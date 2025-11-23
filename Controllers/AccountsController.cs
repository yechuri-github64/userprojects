using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using demo_test_accounts_salesforce_app.Models;
using demo_test_accounts_salesforce_app.Services;

namespace demo_test_accounts_salesforce_app.Controllers
{
 [ApiController]
 [Route("api/[controller]")]
 public class AccountsController : ControllerBase
 {
 private readonly ISalesforceService _salesforceService;
 private readonly ILogger<AccountsController> _logger;

 public AccountsController(ISalesforceService salesforceService, ILogger<AccountsController> logger)
 {
 _salesforceService = salesforceService;
 _logger = logger;
 }

 [HttpGet("{id}")]
 public async Task<IActionResult> Get(string id)
 {
 try
 {
 var account = await _salesforceService.GetAccountAsync(id);
 if (account == null)
 return NotFound();
 return Ok(account);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error fetching account {AccountId}", id);
 return StatusCode(500, "An error occurred while retrieving the account.");
 }
 }

 [HttpPost]
 public async Task<IActionResult> Create([FromBody] Account account)
 {
 try
 {
 var created = await _salesforceService.CreateAccountAsync(account);
 return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error creating account");
 return StatusCode(500, "An error occurred while creating the account.");
 }
 }

 [HttpPut("{id}")]
 public async Task<IActionResult> Update(string id, [FromBody] Account account)
 {
 try
 {
 var updated = await _salesforceService.UpdateAccountAsync(id, account);
 if (!updated) return NotFound();
 return NoContent();
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error updating account {AccountId}", id);
 return StatusCode(500, "An error occurred while updating the account.");
 }
 }

 [HttpDelete("{id}")]
 public async Task<IActionResult> Delete(string id)
 {
 try
 {
 var deleted = await _salesforceService.DeleteAccountAsync(id);
 if (!deleted) return NotFound();
 return NoContent();
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error deleting account {AccountId}", id);
 return StatusCode(500, "An error occurred while deleting the account.");
 }
 }
 }
}
