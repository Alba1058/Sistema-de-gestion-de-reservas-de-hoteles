using Microsoft.AspNetCore.Mvc;
using SGHR.Persistence.Context;

namespace SGHR.Api.Test
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly SGHRContext _context;

        public TestController(SGHRContext context)
        {
            _context = context;
        }

        [HttpGet("check-db")]
        public IActionResult CheckDatabaseConnection()
        {
            try
            {
                bool canConnect = _context.Database.CanConnect();
                return Ok(new { connected = canConnect });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
