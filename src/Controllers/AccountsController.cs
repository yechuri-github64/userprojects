using Microsoft.AspNetCore.Mvc;

namespace MyApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        [HttpPost]
        public IActionResult CreateAccount()
        {
            // Logic for creating an account
            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult GetAccountDetails(int id)
        {
            // Logic for retrieving account details
            return Ok();
        }
    }
}
