using InstituteManagement.Infrastructure;
using InstituteManagement.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InstituteManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public AuthController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByNameAsync(model.UsernameOrEmail)
                       ?? await _userManager.FindByEmailAsync(model.UsernameOrEmail);

            if (user == null)
                return Unauthorized(new { Message = "Invalid username or password" });

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (!result.Succeeded) return Unauthorized(new { Message = "Invalid username or password" });

            // Sign-in succeeded. ASP.NET will set the auth cookie on the response.
            // Return a lightweight UserDto so the client can update UI immediately.
            return Ok(new
            {
                user.Id,
                Username = user.UserName,
                user.Email
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        // Called by Blazor to check current browser's auth cookie
        [HttpGet("whoami")]
        public IActionResult WhoAmI()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return Ok(new
                {
                    IsAuthenticated = true,
                    Username = User.Identity!.Name
                });
            }
            return Ok(new { IsAuthenticated = false, Username = string.Empty });
        }
    }
}