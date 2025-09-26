using FluentValidation;
using FluentValidation.Results;
using InstituteManagement.API.Services;
using InstituteManagement.Infrastructure;
using InstituteManagement.Shared;
using InstituteManagement.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;

namespace InstituteManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : AppControllerBase
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICaptchaValidator _captchaValidator;
        private readonly IValidator<SignInDto> _validator;

        public AuthController(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            ICaptchaValidator captchaValidator,
            IValidator<SignInDto> validator)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _captchaValidator = captchaValidator;
            _validator = validator;
        }

        /// <summary>
        /// Sign-in endpoint - uses FluentValidation + reCAPTCHA verification.
        /// </summary>
        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInDto model)
        {
            // Run FluentValidation
            ValidationResult validationResult = await _validator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                var modelState = new ModelStateDictionary();
                foreach (var error in validationResult.Errors)
                {
                    modelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return ValidationProblem(modelState);
            }

            // CAPTCHA verification (allow dev bypass)
            if (model.RecaptchaToken != "dev-mode")
            {
                var captchaOk = await _captchaValidator.IsCaptchaValid(model.RecaptchaToken, "submit");
                if (!captchaOk)
                    return ValidationError(nameof(model.RecaptchaToken),
                                           MessageKeys.Auth.Keys.RecaptchaTokenRequired.Get(model.Language));
            }

            // locate user by username or email
            var user = await _userManager.FindByNameAsync(model.UsernameOrEmail)
                       ?? await _userManager.FindByEmailAsync(model.UsernameOrEmail);

            if (user == null)
                return Unauthorized(new { Message = MessageKeys.Auth.Keys.InvalidCredentials.Get(model.Language) });

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (!result.Succeeded)
                return Unauthorized(new { Message = MessageKeys.Auth.Keys.InvalidCredentials.Get(model.Language) });

            // sign-in succeeded - return lightweight user info
            // attempt to resolve a FullName claim if available
            var claims = await _userManager.GetClaimsAsync(user);
            var fullName = claims.FirstOrDefault(c => c.Type == "FullName")?.Value
                           ?? user.FullName
                           ?? claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value                           
                           ?? user.UserName;

            return Ok(new
            {
                Message = MessageKeys.Auth.Keys.SignInSuccess.Get(model.Language),
                user.Id,
                Username = user.UserName,
                user.Email,
                FullName = fullName
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { ok = true });
        }

        // Called by Blazor to check current browser's auth cookie
        [HttpGet("whoami")]
        public async Task<IActionResult> WhoAmI()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                //// try to resolve full name from claims
                //AppUser? currentUser = await _userManager.GetUserAsync(User);
                IList<Claim> claims = User.Claims.ToList();
                var fullName = claims.FirstOrDefault(c => c.Type == "FullName")?.Value
                               //?? currentUser.FullName
                               ?? claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

                return Ok(new
                {
                    IsAuthenticated = true,
                    Username = User.Identity!.Name,
                    FullName = fullName ?? string.Empty
                });
            }

            return Ok(new { IsAuthenticated = false, Username = string.Empty, FullName = string.Empty });
        }
    }
}
