using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using test_project_main1.Services;
using test_project_main1.Models;

namespace test_project_main1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NumbersController : ControllerBase
    {
        private readonly INumberService _service;
        private readonly ILogger<NumbersController> _logger;

        public NumbersController(INumberService service, ILogger<NumbersController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var items = await _service.GetAllAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving numbers");
                return StatusCode(500, "An error occurred while retrieving numbers.");
            }
        }

        [HttpGet("divisible")]
        public async Task<IActionResult> GetDivisibleBy5AndLessThan500()
        {
            try
            {
                var items = await _service.GetDivisibleBy5AndLessThan500Async();
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving divisible numbers");
                return StatusCode(500, "An error occurred while retrieving divisible numbers.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var item = await _service.GetByIdAsync(id);
                if (item == null) return NotFound();
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving number by id {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the number.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Number number)
        {
            if (number == null) return BadRequest("Number is required.");
            try
            {
                var created = await _service.CreateAsync(number);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, "Validation error creating number");
                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating number");
                return StatusCode(500, "An error occurred while creating the number.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Number number)
        {
            if (number == null || id != number.Id) return BadRequest("Invalid payload.");
            try
            {
                var updated = await _service.UpdateAsync(number);
                if (!updated) return NotFound();
                return NoContent();
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, "Validation error updating number");
                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating number {Id}", id);
                return StatusCode(500, "An error occurred while updating the number.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);
                if (!deleted) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting number {Id}", id);
                return StatusCode(500, "An error occurred while deleting the number.");
            }
        }
    }
}
