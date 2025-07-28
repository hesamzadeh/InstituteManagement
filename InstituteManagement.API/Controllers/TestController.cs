using InstituteManagement.API.Seed;
using InstituteManagement.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace InstituteManagement.API.Controllers
{
    [ApiController]
    //[Route("api/test")]
    public class TestController : ControllerBase
    {
        private readonly AppDbContext _context;
        public TestController(AppDbContext context) => _context = context;

        [HttpPost("seed")]
        public async Task<IActionResult> Seed()
        {
            await DataSeeder.SeedSampleData(_context);
            return Ok("Seeded");
        }
    }

}
