using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using accounts_management.Models;
using accounts_management.Models;
using accounts_management.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace accounts_management.Controllers;

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
        var accounts = await _service.GetAllAsync();
        return Ok(accounts);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Account>> GetById(int id)
    {
        var account = await _service.GetByIdAsync(id);
        if (account == null)
        {
            return NotFound();
        }
        return Ok(account);
    }

    [HttpPost]
    public async Task<ActionResult<Account>> Create([FromBody] AccountCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }
        var entity = new Account
        {
            Name = dto.Name,
            Email = dto.Email,
            Address = dto.Address,
        };
        var created = await _service.CreateAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPost("bulk")]
    public async Task<ActionResult<IEnumerable<Account>>> CreateBulk(
        [FromBody] IEnumerable<AccountCreateDto> dtos
    )
    {
        if (dtos == null)
        {
            return BadRequest("Request body cannot be null");
        }
        var entities = dtos.Select(d => new Account
            {
                Name = d.Name,
                Email = d.Email,
                Address = d.Address,
            })
            .ToList();
        var created = await _service.CreateManyAsync(entities);
        return StatusCode(201, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Account>> Update(int id, [FromBody] AccountUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }
        var updatedEntity = new Account
        {
            Name = dto.Name,
            Email = dto.Email,
            Address = dto.Address,
        };
        var updated = await _service.UpdateAsync(id, updatedEntity);
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
