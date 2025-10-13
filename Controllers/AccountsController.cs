using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using accounts_management.Models;
using accounts_management.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace accounts_management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountsService _service;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IAccountsService service, ILogger<AccountsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountDto>>> GetAll()
        {
            var accounts = await _service.GetAllAsync();
            return Ok(accounts);
        }

        [HttpGet("{id:int}", Name = nameof(GetById))]
        public async Task<ActionResult<AccountDto>> GetById(int id)
        {
            var account = await _service.GetByIdAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(account);
        }

        [HttpPost]
        public async Task<ActionResult<AccountDto>> Create([FromBody] AccountCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var created = await _service.CreateAsync(dto);
            return CreatedAtRoute(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPost("batch")]
        public async Task<ActionResult<IEnumerable<AccountDto>>> CreateBatch(
            [FromBody] IEnumerable<AccountCreateDto> dtos
        )
        {
            if (dtos == null)
            {
                return BadRequest("Request body must be a JSON array of accounts");
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var list = dtos.ToList();
            if (!list.Any())
            {
                return BadRequest("No accounts provided");
            }

            var created = await _service.CreateBatchAsync(list);
            return StatusCode(201, created);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<AccountDto>> Update(int id, [FromBody] AccountUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null)
            {
                return NotFound();
            }
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
