using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using lucky_number.Models;
using lucky_number.Services;

namespace lucky_number.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeneratedNumbersController : ControllerBase
    {
        private readonly IGeneratedNumberService _service;
        private readonly ILogger<GeneratedNumbersController> _logger;
        private readonly Random _rng = new Random();

        public GeneratedNumbersController(IGeneratedNumberService service, ILogger<GeneratedNumbersController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // Generates a random number between min and max (inclusive) and returns it in JSON without persisting
        [HttpGet("generate")]
        public IActionResult Generate([FromQuery] int min = 5, [FromQuery] int max = 500)
        {
            try
            {
                if (min < 0 || max < 0 || min > max)
                    return BadRequest(new { error = "Invalid range" });

                int value = _rng.Next(min, max + 1);
                return Ok(new { value });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating number");
                return StatusCode(500, new { error = "An error occurred while generating the number" });
            }
        }

        // CRUD endpoints for GeneratedNumber resource (uses EF Core in-memory DB). The generate endpoint above does NOT store numbers.
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
                _logger.LogError(ex, "Error fetching generated numbers");
                return StatusCode(500, new { error = "An error occurred while fetching records" });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var item = await _service.GetAsync(id);
                if (item == null) return NotFound();
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching generated number {Id}", id);
                return StatusCode(500, new { error = "An error occurred while fetching the record" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GeneratedNumber model)
        {
            try
            {
                if (model == null) return BadRequest();
                var created = await _service.CreateAsync(model);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating generated number");
                return StatusCode(500, new { error = "An error occurred while creating the record" });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] GeneratedNumber model)
        {
            try
            {
                if (model == null || id != model.Id) return BadRequest();
                var existing = await _service.GetAsync(id);
                if (existing == null) return NotFound();
                await _service.UpdateAsync(model);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating generated number {Id}", id);
                return StatusCode(500, new { error = "An error occurred while updating the record" });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var existing = await _service.GetAsync(id);
                if (existing == null) return NotFound();
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting generated number {Id}", id);
                return StatusCode(500, new { error = "An error occurred while deleting the record" });
            }
        }
    }
}
