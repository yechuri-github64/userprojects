using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using accounts_sf_sa.Models;
using accounts_sf_sa.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace accounts_sf_sa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly ISalesforceService _sf;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(ISalesforceService sf, ILogger<AccountsController> logger)
        {
            _sf = sf;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMany([FromBody] JsonElement body)
        {
            try
            {
                List<Dictionary<string, object>> accounts = new();

                if (body.ValueKind == JsonValueKind.Array)
                {
                    foreach (var el in body.EnumerateArray())
                    {
                        var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(
                            el.GetRawText()
                        );
                        if (dict != null)
                            accounts.Add(dict);
                    }
                }
                else if (body.ValueKind == JsonValueKind.Object)
                {
                    if (
                        body.TryGetProperty("accounts", out var acctsProp)
                        && acctsProp.ValueKind == JsonValueKind.Array
                    )
                    {
                        foreach (var el in acctsProp.EnumerateArray())
                        {
                            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(
                                el.GetRawText()
                            );
                            if (dict != null)
                                accounts.Add(dict);
                        }
                    }
                    else
                    {
                        var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(
                            body.GetRawText()
                        );
                        if (dict != null)
                            accounts.Add(dict);
                    }
                }
                else
                {
                    return BadRequest(ApiResponse<List<CreateResult>>.Fail("Invalid JSON payload"));
                }

                if (accounts.Count == 0)
                {
                    return BadRequest(ApiResponse<List<CreateResult>>.Fail("No accounts provided"));
                }

                var results = await _sf.CreateMultipleAccountsAsync(accounts);
                return StatusCode(
                    207,
                    ApiResponse<List<CreateResult>>.Ok(results, "Processed multiple accounts")
                );
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error creating accounts");
                return StatusCode(500, ApiResponse<object>.Fail("Internal server error"));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var account = await _sf.GetAccountAsync(id);
                if (account == null)
                {
                    return NotFound(ApiResponse<object>.Fail("Account not found"));
                }
                return Ok(ApiResponse<Dictionary<string, object>>.Ok(account, "Account retrieved"));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error retrieving account {Id}", id);
                return StatusCode(500, ApiResponse<object>.Fail("Internal server error"));
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] JsonElement body)
        {
            try
            {
                if (body.ValueKind != JsonValueKind.Object)
                {
                    return BadRequest(
                        ApiResponse<object>.Fail("Request body must be a JSON object")
                    );
                }
                var fields =
                    JsonSerializer.Deserialize<Dictionary<string, object>>(body.GetRawText())
                    ?? new Dictionary<string, object>();
                if (fields.Count == 0)
                {
                    return BadRequest(ApiResponse<object>.Fail("No fields to update"));
                }
                var ok = await _sf.UpdateAccountAsync(id, fields);
                if (!ok)
                    return NotFound(ApiResponse<object>.Fail("Account not found or update failed"));
                return Ok(ApiResponse<object>.Ok(null, "Account updated"));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error updating account {Id}", id);
                return StatusCode(500, ApiResponse<object>.Fail("Internal server error"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var ok = await _sf.DeleteAccountAsync(id);
                if (!ok)
                    return NotFound(ApiResponse<object>.Fail("Account not found or delete failed"));
                return Ok(ApiResponse<object>.Ok(null, "Account deleted"));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error deleting account {Id}", id);
                return StatusCode(500, ApiResponse<object>.Fail("Internal server error"));
            }
        }
    }
}
