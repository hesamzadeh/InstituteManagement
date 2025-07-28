using InstituteManagement.API.DTOs;
using InstituteManagement.Core.Entities;
using InstituteManagement.Infrastructure;
using InstituteManagement.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InstituteManagement.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;

        public AuthController(UserManager<AppUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup(SignupDto dto)
        {
            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var person = new Person
            {
                NationalCode = dto.SSID,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                UserId = user.Id
            };

            _context.People.Add(person);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
