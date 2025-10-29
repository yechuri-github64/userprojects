using acc_sf_test.Models;
using acc_sf_test.Services;
using Microsoft.AspNetCore.Mvc;

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

 // Create multiple accounts with single JSON (array)
 [HttpPost]
 public async Task<IActionResult> Create([FromBody] IEnumerable<AccountDto> accounts, CancellationToken ct)
 {
 if (accounts == null || !accounts.Any())
 {
 _logger.LogWarning("Create called with empty account list");
 return BadRequest(new { error = "Request must contain an array of account objects" });
 }

 var result = await _accountService.CreateAccountsAsync(accounts, ct);
 return Ok(result);
 }

 // Get a single account
 [HttpGet("{id}")]
 public async Task<IActionResult> Get(string id, CancellationToken ct)
 {
 if (string.IsNullOrWhiteSpace(id)) return BadRequest(new { error = "Id is required" });
 var res = await _accountService.GetAccountAsync(id, ct);
 return Ok(res);
 }

 // Update one account at a time
 [HttpPut("{id}")]
 public async Task<IActionResult> Update(string id, [FromBody] AccountDto account, CancellationToken ct)
 {
 if (string.IsNullOrWhiteSpace(id)) return BadRequest(new { error = "Id is required" });
 var res = await _accountService.UpdateAccountAsync(id, account, ct);
 return Ok(res);
 }

 // Delete
 [HttpDelete("{id}")]
 public async Task<IActionResult> Delete(string id, CancellationToken ct)
 {
 if (string.IsNullOrWhiteSpace(id)) return BadRequest(new { error = "Id is required" });
 var ok = await _accountService.DeleteAccountAsync(id, ct);
 return Ok(new { success = ok });
 }
 }
}